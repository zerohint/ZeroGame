using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ZeroGame.DB
{
    public class Query
    {
        public readonly Collection Collection;

        private readonly List<WhereCondition> _whereConditions = new();
        private readonly List<OrderCondition> _orderConditions = new();
        private int? _limit;

        public Query(Collection collection)
        {
            this.Collection = collection;
        }

        public Query Where(string fieldPath, Operator @operator, object value)
        {
            _whereConditions.Add(new WhereCondition(fieldPath, @operator, value));
            return this;
        }

        public Query OrderBy(string fieldPath, bool descending = false)
        {
            _orderConditions.Add(new OrderCondition(fieldPath, descending));
            return this;
        }

        public Query Limit(int limit)
        {
            _limit = limit;
            return this;
        }

        public void GetSnapshot(Action<DBResponse> onComplete)
        {
            var structuredQuery = new Dictionary<string, object>
            {
                ["from"] = new[]
                {
                    new Dictionary<string, object> { ["collectionId"] = Collection.Id }
                }
            };

            if (_whereConditions.Count > 0)
            {
                structuredQuery["where"] = new Dictionary<string, object>
                {
                    ["compositeFilter"] = new Dictionary<string, object>
                    {
                        ["op"] = "AND",
                        ["filters"] = _whereConditions.Select(w => new Dictionary<string, object>
                        {
                            ["fieldFilter"] = new Dictionary<string, object>
                            {
                                ["field"] = new Dictionary<string, object> { ["fieldPath"] = w.FieldPath },
                                ["op"] = w.Operator.ToString(),
                                ["value"] = FirestoreHelper.ConvertToFirestoreValue(w.Value)
                            }
                        }).ToArray()
                    }
                };
            }

            if (_orderConditions.Count > 0)
            {
                structuredQuery["orderBy"] = _orderConditions.Select(o => new Dictionary<string, object>
                {
                    ["field"] = new Dictionary<string, object> { ["fieldPath"] = o.FieldPath },
                    ["direction"] = o.Descending ? "DESCENDING" : "ASCENDING"
                }).ToArray();
            }

            if (_limit.HasValue)
                structuredQuery["limit"] = _limit.Value;

            TheSingleton.Instance.StartCoroutine(RunQueryCR(Collection.Id, structuredQuery, onComplete));
        }




        private IEnumerator RunQueryCR(string collection, Dictionary<string, object> structuredQuery, Action<DBResponse> onComplete)
        {
            string url = $"{ZGameManager.BASE_URL}projects/{ZGameManager.Instance.ProjectId}/databases/(default)/documents:runQuery?key={ZGameManager.Instance.ApiKey}";

            var body = new Dictionary<string, object>
            {
                ["structuredQuery"] = structuredQuery
            };

            var json = JsonConvert.SerializeObject(body);
            using UnityWebRequest request = new (url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            if (!string.IsNullOrEmpty(ZGame.DB.IdToken))
                request.SetRequestHeader("Authorization", $"Bearer {ZGame.DB.IdToken}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var raw = request.downloadHandler.text;
                onComplete(new DBResponse(request));
            }
            else
            {
                UnityEngine.Debug.LogError($"Query failed: {request.responseCode} - {request.error}");
                onComplete(null);
            }
        }






















        public enum Operator
        {
            EQUAL, // ==
            NOT_EQUAL, // !=
            LESS_THAN, // <
            LESS_THAN_OR_EQUAL, // <=
            GREATER_THAN, // >
            GREATER_THAN_OR_EQUAL, // >=
        }

        private struct WhereCondition
        {
            public string FieldPath;
            public Operator Operator;
            public object Value;

            public WhereCondition(string fieldPath, Operator op, object value)
            {
                FieldPath = fieldPath;
                Operator = op;
                Value = value;
            }
        }

        private struct OrderCondition
        {
            public string FieldPath;
            public bool Descending;

            public OrderCondition(string fieldPath, bool descending)
            {
                FieldPath = fieldPath;
                Descending = descending;
            }
        }
    }
}