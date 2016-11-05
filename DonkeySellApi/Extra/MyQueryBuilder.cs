using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DonkeySellApi.Extra
{
    public interface IMyQueryBuilder
    {
        Task<string> BuildQuery(string queryBulk);
    }

    public class MyQueryBuilder : IMyQueryBuilder
    {

        private class QueryParts
        {
            public string ColumnName { get; set; }

            public string MinValue { get; set; }

            public string MaxValue { get; set; }

            public bool UseLike { get; set; }

            public bool UseOr { get; set; }
        }

        private string queryStart = "select * from Product where ";

        public async Task<string> BuildQuery(string queryBulk)
        {
            if (CheckForBadUser(queryBulk))
                return null;

            List<QueryParts> queryPartsList = GetQueryParts(queryBulk);

            string query = AssembleQuery(queryPartsList);

            return query;
        }

        private bool CheckForBadUser(string queryBulk)
        {
            var badWords = new[] { "insert", "update", "alter", "create", "drop", "delete", "--" };
            foreach (var badWord in badWords)
            {
                if (queryBulk.ToLower().Contains(badWord))
                    return true;
            }

            return false;
        }

        private string AssembleQuery(List<QueryParts> queryPartsList)
        {
            StringBuilder qBuilder = new StringBuilder();
            qBuilder.Append(queryStart);

            for (int i = 0; i < queryPartsList.Count; i++)
            {
                if (i==0 || !queryPartsList[i - 1].UseOr)
                    qBuilder.Append(" ( ");
                if (i == queryPartsList.Count - 1)
                {
                    BuildQueryCondition(queryPartsList, i, ref qBuilder);
                    qBuilder.Append(" ) ");
                }
                else
                {
                    BuildQueryCondition(queryPartsList, i, ref qBuilder);
                    qBuilder.Append(queryPartsList[i].UseOr ? " Or " : " ) and ");
                }
            }

            return qBuilder.ToString();
        }

        private void BuildQueryCondition(List<QueryParts> queryPartsList, int i, ref StringBuilder qBuilder)
        {
            var part = queryPartsList[i];
            if (part.UseLike)
                qBuilder.Append(part.ColumnName + " like '%" + part.MinValue + "%'");
            else if (part.MinValue == part.MaxValue)
                qBuilder.Append(part.ColumnName + " ='" + part.MinValue + "'");
            else
                qBuilder.Append(part.ColumnName + " >='" + part.MinValue + "' and " + part.ColumnName + " <='" +
                                part.MaxValue + "'");
        }

        private List<QueryParts> GetQueryParts(string queryBulk)
        {
            var queryPartsString = queryBulk.Split(';');
            List<QueryParts> queryPartsList = new List<QueryParts>();
            foreach (var s in queryPartsString)
            {
                BuildQueryPartObject(s, ref queryPartsList);
            }

            return queryPartsList;
        }

        private void BuildQueryPartObject(string s, ref List<QueryParts> queryPartsList)
        {
            var elements = s.Split(',');
            switch (elements.Length)
            {
                case 2:
                    queryPartsList.Add(new QueryParts()
                    {
                        ColumnName = elements[0],
                        MaxValue = elements[1],
                        MinValue = elements[1],
                        UseLike = false,
                        UseOr = false,
                    });
                    break;
                case 3:
                    queryPartsList.Add(new QueryParts()
                    {
                        ColumnName = elements[0],
                        MinValue = elements[1],
                        MaxValue = elements[2],
                        UseLike = false,
                        UseOr = false
                    });
                    break;

                case 4:
                    queryPartsList.Add(new QueryParts()
                    {
                        ColumnName = elements[0],
                        MinValue = elements[1],
                        MaxValue = elements[2],
                        UseLike = true,
                        UseOr = false
                    });
                    break;
                case 5:
                    queryPartsList.Add(new QueryParts()
                    {
                        ColumnName = elements[0],
                        MinValue = elements[1],
                        MaxValue = elements[2],
                        UseLike = true,
                        UseOr = true
                    });
                    break;
            }
        }
    }
}