using System.Collections.Generic;
using System.Linq;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;

namespace EqualityInformationApi.V1.Infrastructure
{
    public class DynamoDbNullConverter<T> : IPropertyConverter
    {
        public DynamoDBEntry ToEntry(object value)
        {
            DynamoDBEntry entry;

            if (typeof(T) == typeof(string))
            {
                entry = new Primitive { Value = value };
            }
            else
            {
                if ((List<string>) value == null || !((List<string>) value).Any())
                {
                    return new DynamoDBList();
                }

                var stringList = value as List<string>;
                entry = new DynamoDBList(stringList.Select(x => new Primitive { Value = x }));
            }

            return entry;
        }

        public object FromEntry(DynamoDBEntry entry)
        {
            if (typeof(T) == typeof(string))
            {
                if (entry == null || entry.AsDynamoDBNull() != null)
                    return string.Empty;

                return entry.AsString();
            }
            else
            {
                if (entry == null || entry.AsDynamoDBNull() != null)
                    return new List<string>();

                return entry.AsListOfString();
            }
        }
    }
}
