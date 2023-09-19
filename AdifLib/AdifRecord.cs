using System;
using System.Collections.Generic;
using System.Linq;

namespace M0LTE.AdifLib
{
    public class AdifRecord
    {
        internal AdifRecord() { }

        public Dictionary<string, string> Fields { get; private set; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        protected void SetField(string fieldName, string value) => Fields[fieldName] = value;

        protected virtual string EndMarker { get; }

        public override string ToString() => string.Join("\n", Fields.Where(f => !string.IsNullOrWhiteSpace(f.Value)).Select(f => $"<{f.Key}:{f.Value.Length}>{f.Value}")) + "\n" + EndMarker;

        internal static bool TryParse(string record, out AdifRecord adifRecord, out string error)
        {
            adifRecord = new AdifRecord();

            ParseState p = ParseState.LookingForStartOfRecord;
            var fieldNameBuffer = new List<char>();
            var fieldLenBuffer = new List<char>();
            var dataBuffer = new List<char>();

            try
            {
                for (int i = 0; i < record.Length; i++)
                {
                    if (p == ParseState.LookingForStartOfRecord)
                    {
                        if (record[i] == '<')
                        {
                            p = ParseState.FieldName;
                        }
                        else continue;
                    }
                    else if (p == ParseState.FieldName)
                    {
                        if (record[i] == ':')
                        {
                            p = ParseState.FieldLen;
                        }
                        else
                        {
                            fieldNameBuffer.Add(record[i]);
                        }
                    }
                    else if (p == ParseState.FieldLen)
                    {
                        if (record[i] == '>')
                        {
                            p = ParseState.Data;
                        }
                        else
                        {
                            fieldLenBuffer.Add(record[i]);
                        }
                    }
                    else if (p == ParseState.Data)
                    {
                        int fieldLen = int.Parse(new String(fieldLenBuffer.ToArray()));
                        if (dataBuffer.Count == fieldLen)
                        {
                            string fieldName = new string(fieldNameBuffer.ToArray());
                            string data = new string(dataBuffer.ToArray());

                            adifRecord.Fields.Add(fieldName, data);

                            fieldNameBuffer.Clear();
                            fieldLenBuffer.Clear();
                            dataBuffer.Clear();
                            p = ParseState.LookingForStartOfRecord;
                        }
                        else
                        {
                            dataBuffer.Add(record[i]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

            error = null;
            return true;
        }
    }
    internal enum ParseState
    {
        LookingForStartOfRecord, FieldName, FieldLen, Data
    }
}