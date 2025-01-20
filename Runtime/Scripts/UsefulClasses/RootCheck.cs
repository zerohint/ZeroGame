namespace ZeroGame
{
    public static class RootCheck
    {
        // not done
#if false && UNITY_ANDROID
        public static bool IsDeviceRooted() => CheckSuBinary() || CheckRootManagementApps() || CheckCommonRootFiles();


        /// <summary>
        /// SU binary kontrol
        /// </summary>
        /// <returns></returns>
        private static bool CheckSuBinary()
        {
            try
            {
                using (AndroidJavaObject process = new AndroidJavaObject("java.lang.ProcessBuilder", new string[] { "sh", "-c", "type su" }))
                {
                    process.Call("start");
                    AndroidJavaObject inputStream = process.Get<AndroidJavaObject>("inputStream");
                    string output = new AndroidJavaObject("java.io.BufferedReader", new AndroidJavaObject("java.io.InputStreamReader", inputStream)).Call<string>("readLine");
                    if (!string.IsNullOrEmpty(output) && output.Contains("su"))
                    {
                        return true;
                    }
                }
            }
            catch (System.Exception)
            {
                // Hata, rootlu olmayabilir.
            }
            return false;
        }

        /// <summary>
        /// Root yonetim uygulamalari kontrolu
        /// </summary>
        /// <returns></returns>
        private static bool CheckRootManagementApps()
        {
            return false;

            // DEBUG: Fatal om.ttcoin.sweet java_vm_ext.cc:598] JNI DETECTED ERROR IN APPLICATION: can't make objects of type android.content.pm.PackageManager: 0x6fc283f8
            string[] rootApps = {
            "com.noshufou.android.su",
            "com.noshufou.android.su.elite",
            "eu.chainfire.supersu",
            "com.koushikdutta.superuser",
            "com.zachspong.temprootremovejb",
            "com.ramdroid.appquarantine"
        };

            try
            {
                using (AndroidJavaObject packageManager = new AndroidJavaObject("android.content.pm.PackageManager"))
                {
                    AndroidJavaObject GetUnityActivity()
                    {
                        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                        {
                            return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                        }
                    }

                    AndroidJavaObject context = GetUnityActivity().Call<AndroidJavaObject>("getApplicationContext");

                    foreach (string app in rootApps)
                    {
                        try
                        {
                            AndroidJavaObject appInfo = packageManager.Call<AndroidJavaObject>("getApplicationInfo", context, app, 0);
                            if (appInfo != null)
                            {
                                return true;
                            }
                        }
                        catch (System.Exception)
                        {
                            // Uygulama bulunamadi, rootlu olmayabilir.
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                // Hata, rootlu olmayabilir.
            }

            return false;
        }

        /// <summary>
        /// Common root dosyalari kontrolu
        /// </summary>
        /// <returns></returns>
        private static bool CheckCommonRootFiles()
        {
            string[] rootFiles = {
            "/data/local/su",
            "/data/local/bin/su",
            "/data/local/xbin/su",
            "/sbin/su",
            "/su/bin/su",
            "/system/bin/su",
            "/system/sd/xbin/su",
            "/system/xbin/su",
            "/system/app/Superuser.apk",
            "/system/etc/init.d/99SuperSUDaemon"
        };

            foreach (string file in rootFiles)
            {
                try
                {
                    AndroidJavaObject fileObject = new AndroidJavaObject("java.io.File", file);
                    if (fileObject.Call<bool>("exists"))
                    {
                        return true;
                    }
                }
                catch (System.Exception)
                {
                    // Hata, may not be rooted.
                }
            }
            return false;
        }
#else
        public static bool IsDeviceRooted() => false;
#endif
    }
}