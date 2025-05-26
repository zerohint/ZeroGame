using System;
using System.Collections.Generic;

namespace ZeroGame.DB
{
    public class Collection
    {
        public readonly string Id;

        internal readonly DB DB;
        internal readonly Dictionary<string, DBResponse> documentCache = new();

        public Collection(DB db, string id)
        {
            this.Id = id;
            DB = db;
        }

        public Document Document(string id)
        {
            if (id.IsNullOrEmpty())
                throw new ArgumentNullException("id");
            return new(this, id);
        }

        public Query CreateQuery() => new (this);

        public void Add(Dictionary<string, object> data, Action<DBResponse> onComplete)
        {
            DB.SendRequest(Id, Method.POST, data, onComplete);
        }
        public void Add(string documentId, Dictionary<string, object> data, Action<DBResponse> onComplete)
        {
            DB.SendRequest($"{Id}/{documentId}", Method.PATCH, data, onComplete);
        }

        public void Delete(string documentId, Action<DBResponse> onComplete)
        {
            DB.SendRequest(Id + "/" + documentId, Method.DELETE, null, onComplete);
        }

    }
}