using System.Collections.Generic;
using System;
using System.Linq;

namespace ZeroGame.DB
{
    public class Document
    {
        public readonly Collection Collection;
        public readonly string Id;

        public Document(Collection collection, string id)
        {
            this.Collection = collection;
            this.Id = id;
        }

        /// <summary>
        /// Is document exists in the collection
        /// </summary>
        /// <param name="action"></param>
        /// <exception cref="Exception"></exception>
        public void IsExists(Action<bool> action)
        {
            GetSnapshot((response) =>
            {
                if (response.IsSuccess)
                {
                    action?.Invoke(true);

                }
                else
                {
                    if(response.RawBody.Contains("\"code\": 404"))
                    {
                        action?.Invoke(false);
                    }
                    else
                    {
                        throw new Exception("Couldn't check if exists: " + response.Error);
                    }
                }
            });
        }

        public void GetSnapshot(Action<DBResponse> onComplete, bool tryCache = false)
        {
            if (tryCache && Collection.documentCache.ContainsKey(Id))
            {
                onComplete.Invoke(Collection.documentCache[Id]);
            }
            Collection.DB.SendRequest(Collection.Id + "/" + Id, Method.GET, null, r =>
            {
                if (r.IsSuccess)
                {
                    Collection.documentCache.AddOrUpdate(r.AsSnapshot.Id, r);
                }
                onComplete.Invoke(r);
            });
        }

        public void Set(object obj, Action<DBResponse> onComplete)
        {
            // set class to firebase
            throw new System.NotImplementedException();
        }

        public void Update(Dictionary<string, object> dictionary, Action<DBResponse> onComplete)
        {
            Collection.DB.SendRequest(Collection.Id + "/" + Id, Method.PATCH, dictionary, onComplete);
        }


        public void Listen(Action<DocumentSnapshot> listener) => throw new NotImplementedException();
        public void StopListening() => throw new NotImplementedException();
    }
}