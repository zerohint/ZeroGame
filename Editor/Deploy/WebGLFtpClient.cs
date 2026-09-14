using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

// FtpWebRequest is obsolete in .NET 6+ but is still the only FTP client shipped with the
// Editor's runtime, and adding a NuGet dependency to the package for one button is worse.
#pragma warning disable SYSLIB0014

namespace ZeroGame.Editor
{
    /// <summary>
    /// Minimal recursive FTP uploader for the WebGL "Build and Deploy" button.
    /// Every failure throws - the caller turns that into an error log, nothing fails silently.
    /// </summary>
    internal sealed class WebGLFtpClient
    {
        private readonly string baseUrl;
        private readonly NetworkCredential credentials;
        private readonly bool useFtps;
        private readonly bool passive;

        /// <summary>Remote directories already created (or confirmed) during this session.</summary>
        private readonly HashSet<string> knownDirectories = new();

        internal WebGLFtpClient(WebGLDeploySettings settings)
        {
            baseUrl = $"ftp://{settings.Host.Trim()}:{settings.Port}/{NormalizeRemotePath(settings.RemoteDirectory)}";
            credentials = new NetworkCredential(settings.Username.Trim(), settings.Password);
            useFtps = settings.UseFtps;
            passive = settings.PassiveMode;
        }

        /// <summary>Lists the target directory. Used by the Test Connection button.</summary>
        internal void TestConnection()
        {
            var request = CreateRequest("", WebRequestMethods.Ftp.ListDirectory);
            using var response = (FtpWebResponse)request.GetResponse();
            using var reader = new StreamReader(response.GetResponseStream());
            reader.ReadToEnd();
        }

        /// <summary>
        /// Uploads <paramref name="localFile"/> to <paramref name="remoteRelativePath"/>
        /// (forward-slash separated, relative to the configured remote directory).
        /// </summary>
        internal void UploadFile(string localFile, string remoteRelativePath)
        {
            var lastSlash = remoteRelativePath.LastIndexOf('/');
            if (lastSlash > 0)
                EnsureDirectory(remoteRelativePath.Substring(0, lastSlash));

            var request = CreateRequest(remoteRelativePath, WebRequestMethods.Ftp.UploadFile);
            request.ContentLength = new FileInfo(localFile).Length;

            using (var source = File.OpenRead(localFile))
            using (var destination = request.GetRequestStream())
            {
                source.CopyTo(destination, 81920);
            }

            using var response = (FtpWebResponse)request.GetResponse();
            if (response.StatusCode != FtpStatusCode.ClosingData &&
                response.StatusCode != FtpStatusCode.FileActionOK &&
                response.StatusCode != FtpStatusCode.CommandOK)
            {
                throw new IOException(
                    $"FTP refused '{remoteRelativePath}': {response.StatusCode} {response.StatusDescription}");
            }
        }

        /// <summary>
        /// File names (no paths) directly inside <paramref name="remoteRelativeDirectory"/>.
        /// An empty list means "directory is empty"; a missing directory throws like everything else.
        /// </summary>
        internal List<string> ListFileNames(string remoteRelativeDirectory)
        {
            var names = new List<string>();

            var request = CreateRequest(remoteRelativeDirectory, WebRequestMethods.Ftp.ListDirectory);
            using var response = (FtpWebResponse)request.GetResponse();
            using var reader = new StreamReader(response.GetResponseStream());

            while (reader.ReadLine() is { } line)
            {
                // NLST answers with bare names on some servers and full paths on others.
                var name = line.Trim().TrimEnd('\r');
                var lastSlash = name.LastIndexOf('/');
                if (lastSlash >= 0) name = name.Substring(lastSlash + 1);

                if (name.Length == 0 || name == "." || name == "..") continue;

                names.Add(name);
            }

            return names;
        }

        /// <summary>Deletes one remote file. Throws if the server refuses.</summary>
        internal void DeleteFile(string remoteRelativePath)
        {
            var request = CreateRequest(remoteRelativePath, WebRequestMethods.Ftp.DeleteFile);
            using var response = (FtpWebResponse)request.GetResponse();
        }

        /// <summary>
        /// Creates every segment of a relative remote directory path, ignoring "already exists".
        /// </summary>
        private void EnsureDirectory(string remoteRelativeDirectory)
        {
            var accumulated = "";

            foreach (var segment in remoteRelativeDirectory.Split('/'))
            {
                if (segment.Length == 0) continue;

                accumulated = accumulated.Length == 0 ? segment : accumulated + "/" + segment;
                if (!knownDirectories.Add(accumulated)) continue;

                try
                {
                    var request = CreateRequest(accumulated, WebRequestMethods.Ftp.MakeDirectory);
                    using var response = (FtpWebResponse)request.GetResponse();
                }
                catch (WebException exception)
                {
                    // 550 / 521 = directory already there, which is the normal case on re-deploy.
                    if (exception.Response is FtpWebResponse ftpResponse &&
                        (ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable ||
                         ftpResponse.StatusCode == FtpStatusCode.ActionNotTakenFilenameNotAllowed))
                    {
                        continue;
                    }

                    throw;
                }
            }
        }

        private FtpWebRequest CreateRequest(string remoteRelativePath, string method)
        {
            var url = string.IsNullOrEmpty(remoteRelativePath)
                ? baseUrl
                : baseUrl + EscapeRemotePath(remoteRelativePath);

            var request = (FtpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Credentials = credentials;
            request.EnableSsl = useFtps;
            request.UsePassive = passive;
            request.UseBinary = true;
            request.KeepAlive = true;
            request.Timeout = 30000;
            request.ReadWriteTimeout = 120000;
            return request;
        }

        /// <summary>
        /// Escapes each path segment separately so that '/' stays a separator while spaces and
        /// other unsafe characters in file names are encoded.
        /// </summary>
        private static string EscapeRemotePath(string remoteRelativePath)
        {
            var segments = remoteRelativePath.Split('/');
            for (var i = 0; i < segments.Length; i++)
                segments[i] = Uri.EscapeDataString(segments[i]);

            return string.Join("/", segments);
        }

        /// <summary>"/public_html" or "public_html/" -> "public_html/", "/" -> "".</summary>
        private static string NormalizeRemotePath(string directory)
        {
            var trimmed = (directory ?? "").Replace('\\', '/').Trim().Trim('/');
            return trimmed.Length == 0 ? "" : trimmed + "/";
        }

        /// <summary>Enumerates a built player as (absolute file, remote relative path) pairs.</summary>
        internal static List<(string local, string remote)> CollectFiles(string rootDirectory)
        {
            var files = new List<(string, string)>();
            var rootFullPath = Path.GetFullPath(rootDirectory);

            foreach (var file in Directory.GetFiles(rootFullPath, "*", SearchOption.AllDirectories))
            {
                var name = Path.GetFileName(file);
                if (name == ".DS_Store" || name.EndsWith(".meta", StringComparison.Ordinal))
                    continue;

                var relative = file.Substring(rootFullPath.Length)
                                   .TrimStart(Path.DirectorySeparatorChar, '/')
                                   .Replace('\\', '/');

                files.Add((file, relative));
            }

            if (files.Count == 0)
                Debug.LogWarning($"[ZeroGame] Nothing to upload, '{rootFullPath}' contains no files.");

            return files;
        }
    }
}
