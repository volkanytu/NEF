using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;


namespace NEF.Library.Business
{
    public static class DocumentMerge
    {
        private static readonly Regex instructionRegEx = new Regex("^[\\s]*MERGEFIELD[\\s]+(?<name>[#\\w]*){1}               # This retrieves the field's name (Named Capture Group -> name)\r\n                            [\\s]*(\\\\\\*[\\s]+(?<Format>[\\w]*){1})?                # Retrieves field's format flag (Named Capture Group -> Format)\r\n                            [\\s]*(\\\\b[\\s]+[\"]?(?<PreText>[^\\\\]*){1})?         # Retrieves text to display before field data (Named Capture Group -> PreText)\r\n                                                                                # Retrieves text to display after field data (Named Capture Group -> PostText)\r\n                            [\\s]*(\\\\f[\\s]+[\"]?(?<PostText>[^\\\\]*){1})?", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace | RegexOptions.CultureInvariant);

        public static byte[] WordDokumanOlustur(string sablon, DataSet dataset, Dictionary<string, string> degerler)
        {
            byte[] buffer = File.ReadAllBytes(sablon);
            string[] switches = (string[])null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(buffer, 0, buffer.Length);
                using (WordprocessingDocument docx = WordprocessingDocument.Open((Stream)memoryStream, true))
                {
                    DocumentMerge.ConvertFieldCodes((OpenXmlElement)docx.MainDocumentPart.Document);
                    foreach (SimpleField field in docx.MainDocumentPart.Document.Descendants<SimpleField>())
                    {
                        string fieldName = DocumentMerge.GetFieldName(field, out switches);
                        if (!string.IsNullOrEmpty(fieldName) && fieldName.StartsWith("TBL_"))
                        {
                            TableRow firstParent1 = DocumentMerge.GetFirstParent<TableRow>((OpenXmlElement)field);
                            if (firstParent1 != null)
                            {
                                Table firstParent2 = DocumentMerge.GetFirstParent<Table>((OpenXmlElement)firstParent1);
                                if (firstParent2 != null)
                                {
                                    string nameFromFieldName = DocumentMerge.GetTableNameFromFieldName(fieldName);
                                    if (dataset != null && dataset.Tables.Contains(nameFromFieldName) && dataset.Tables[nameFromFieldName].Rows.Count != 0)
                                    {
                                        DataTable dataTable = dataset.Tables[nameFromFieldName];
                                        List<TableCellProperties> list1 = new List<TableCellProperties>();
                                        List<string> list2 = new List<string>();
                                        List<string> list3 = new List<string>();
                                        List<SimpleField> list4 = new List<SimpleField>();
                                        foreach (TableCell tableCell in firstParent1.Descendants<TableCell>())
                                        {
                                            list1.Add(tableCell.GetFirstChild<TableCellProperties>());
                                            Paragraph firstChild1 = tableCell.GetFirstChild<Paragraph>();
                                            if (firstChild1 != null)
                                            {
                                                ParagraphProperties firstChild2 = firstChild1.GetFirstChild<ParagraphProperties>();
                                                if (firstChild2 != null)
                                                    list3.Add(firstChild2.OuterXml);
                                                else
                                                    list3.Add((string)null);
                                            }
                                            else
                                                list3.Add((string)null);
                                            string str = string.Empty;
                                            SimpleField simpleField = (SimpleField)null;
                                            using (IEnumerator<SimpleField> enumerator = tableCell.Descendants<SimpleField>().GetEnumerator())
                                            {
                                                if (enumerator.MoveNext())
                                                {
                                                    SimpleField current = enumerator.Current;
                                                    simpleField = current;
                                                    str = DocumentMerge.GetColumnNameFromFieldName(DocumentMerge.GetFieldName(current, out switches));
                                                }
                                            }
                                            list2.Add(str);
                                            if (str != "")
                                                list4.Add(simpleField);
                                        }
                                        TableRowProperties firstChild = firstParent1.GetFirstChild<TableRowProperties>();
                                        foreach (DataRow dataRow in (InternalDataCollectionBase)dataTable.Rows)
                                        {
                                            TableRow tableRow = new TableRow();
                                            if (firstChild != null)
                                                tableRow.Append(new OpenXmlElement[1]
                        {
                          (OpenXmlElement) new TableRowProperties(firstChild.OuterXml)
                        });
                                            for (int index = 0; index < list1.Count; ++index)
                                            {
                                                TableCellProperties tableCellProperties = new TableCellProperties(list1[index].OuterXml);
                                                TableCell tableCell = new TableCell();
                                                tableCell.Append(new OpenXmlElement[1]
                        {
                          (OpenXmlElement) tableCellProperties
                        });
                                                Paragraph paragraph = new Paragraph(new OpenXmlElement[1]
                        {
                          (OpenXmlElement) new ParagraphProperties(list3[index])
                        });
                                                tableCell.Append(new OpenXmlElement[1]
                        {
                          (OpenXmlElement) paragraph
                        });
                                                try
                                                {
                                                    if (!string.IsNullOrEmpty(list2[index]))
                                                    {
                                                        if (!dataTable.Columns.Contains(list2[index]))
                                                            throw new Exception(string.Format("Unable to complete template: column name '{0}' is unknown in parameter tables !", (object)list2[index]));
                                                        if (!dataRow.IsNull(list2[index]))
                                                        {
                                                            string text = dataRow[list2[index]].ToString();
                                                            paragraph.Append(new OpenXmlElement[1]
                              {
                                (OpenXmlElement) DocumentMerge.GetRunElementForText(text, list4[index])
                              });
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                }
                                                tableRow.Append(new OpenXmlElement[1]
                        {
                          (OpenXmlElement) tableCell
                        });
                                            }
                                            firstParent2.Append(new OpenXmlElement[1]
                      {
                        (OpenXmlElement) tableRow
                      });
                                        }
                                        firstParent1.Remove();
                                    }
                                }
                            }
                        }
                    }
                    foreach (SimpleField field in docx.MainDocumentPart.Document.Descendants<SimpleField>())
                    {
                        string fieldName = DocumentMerge.GetFieldName(field, out switches);
                        if (!string.IsNullOrEmpty(fieldName) && fieldName.StartsWith("TBL_"))
                        {
                            TableRow firstParent1 = DocumentMerge.GetFirstParent<TableRow>((OpenXmlElement)field);
                            if (firstParent1 != null)
                            {
                                Table firstParent2 = DocumentMerge.GetFirstParent<Table>((OpenXmlElement)firstParent1);
                                if (firstParent2 != null)
                                {
                                    string nameFromFieldName = DocumentMerge.GetTableNameFromFieldName(fieldName);
                                    if ((dataset == null || !dataset.Tables.Contains(nameFromFieldName) || dataset.Tables[nameFromFieldName].Rows.Count == 0) && Enumerable.Contains<string>((IEnumerable<string>)switches, "dt"))
                                        firstParent2.Remove();
                                }
                            }
                        }
                    }
                    DocumentMerge.FillWordFieldsInElement(docx, degerler, (OpenXmlElement)docx.MainDocumentPart.Document);
                    ((OpenXmlPartRootElement)docx.MainDocumentPart.Document).Save();
                    foreach (HeaderPart headerPart in docx.MainDocumentPart.HeaderParts)
                    {
                        DocumentMerge.ConvertFieldCodes((OpenXmlElement)headerPart.Header);
                        DocumentMerge.FillWordFieldsInElement(docx, degerler, (OpenXmlElement)headerPart.Header);
                        ((OpenXmlPartRootElement)headerPart.Header).Save();
                    }
                    foreach (FooterPart footerPart in docx.MainDocumentPart.FooterParts)
                    {
                        DocumentMerge.ConvertFieldCodes((OpenXmlElement)footerPart.Footer);
                        DocumentMerge.FillWordFieldsInElement(docx, degerler, (OpenXmlElement)footerPart.Footer);
                        ((OpenXmlPartRootElement)footerPart.Footer).Save();
                    }
                }
                memoryStream.Seek(0L, SeekOrigin.Begin);
                return memoryStream.ToArray();
            }
        }

        internal static string[] ApplyFormatting(string format, string fieldValue, string preText, string postText)
        {
            string[] strArray = new string[3];
            if ("UPPER".Equals(format))
            {
                strArray[0] = fieldValue.ToUpper(CultureInfo.CurrentCulture);
                strArray[1] = preText.ToUpper(CultureInfo.CurrentCulture);
                strArray[2] = postText.ToUpper(CultureInfo.CurrentCulture);
            }
            else if ("LOWER".Equals(format))
            {
                strArray[0] = fieldValue.ToLower(CultureInfo.CurrentCulture);
                strArray[1] = preText.ToLower(CultureInfo.CurrentCulture);
                strArray[2] = postText.ToLower(CultureInfo.CurrentCulture);
            }
            else if ("FirstCap".Equals(format))
            {
                if (!string.IsNullOrEmpty(fieldValue))
                {
                    strArray[0] = fieldValue.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture);
                    if (fieldValue.Length > 1)
                        strArray[0] = strArray[0] + fieldValue.Substring(1).ToLower(CultureInfo.CurrentCulture);
                }
                if (!string.IsNullOrEmpty(preText))
                {
                    strArray[1] = preText.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture);
                    if (fieldValue.Length > 1)
                        strArray[1] = strArray[1] + preText.Substring(1).ToLower(CultureInfo.CurrentCulture);
                }
                if (!string.IsNullOrEmpty(postText))
                {
                    strArray[2] = postText.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture);
                    if (fieldValue.Length > 1)
                        strArray[2] = strArray[2] + postText.Substring(1).ToLower(CultureInfo.CurrentCulture);
                }
            }
            else if ("Caps".Equals(format))
            {
                strArray[0] = DocumentMerge.ToTitleCase(fieldValue);
                strArray[1] = DocumentMerge.ToTitleCase(preText);
                strArray[2] = DocumentMerge.ToTitleCase(postText);
            }
            else
            {
                strArray[0] = fieldValue;
                strArray[1] = preText;
                strArray[2] = postText;
            }
            return strArray;
        }

        internal static void ExecuteSwitches(OpenXmlElement element, string[] switches)
        {
            if (switches == null || Enumerable.Count<string>((IEnumerable<string>)switches) == 0)
                return;
            if (Enumerable.Contains<string>((IEnumerable<string>)switches, "dp"))
            {
                Paragraph firstParent = DocumentMerge.GetFirstParent<Paragraph>(element);
                if (firstParent == null)
                    return;
                firstParent.Remove();
            }
            else if (Enumerable.Contains<string>((IEnumerable<string>)switches, "dr"))
            {
                TableRow firstParent = DocumentMerge.GetFirstParent<TableRow>(element);
                if (firstParent == null)
                    return;
                firstParent.Remove();
            }
            else
            {
                if (!Enumerable.Contains<string>((IEnumerable<string>)switches, "dt"))
                    return;
                Table firstParent = DocumentMerge.GetFirstParent<Table>(element);
                if (firstParent != null)
                    firstParent.Remove();
            }
        }

        internal static void FillWordFieldsInElement(WordprocessingDocument docx, Dictionary<string, string> values, OpenXmlElement element)
        {
            Dictionary<SimpleField, string[]> dictionary = new Dictionary<SimpleField, string[]>();
            foreach (SimpleField index in element.Descendants<SimpleField>())
            {
                string[] switches;
                string[] options;
                string fieldNameWithOptions = DocumentMerge.GetFieldNameWithOptions(index, out switches, out options);
                if (!string.IsNullOrEmpty(fieldNameWithOptions) && values.ContainsKey(fieldNameWithOptions))
                {
                    if (values.ContainsKey(fieldNameWithOptions) && !string.IsNullOrEmpty(values[fieldNameWithOptions]))
                    {
                        string[] strArray = DocumentMerge.ApplyFormatting(options[0], ((object)values[fieldNameWithOptions]).ToString(), options[1], options[2]);
                        if (!string.IsNullOrEmpty(options[1]))
                            index.Parent.InsertBeforeSelf<Paragraph>(DocumentMerge.GetPreOrPostParagraphToInsert(strArray[1], index));
                        if (!string.IsNullOrEmpty(options[2]))
                            index.Parent.InsertAfterSelf<Paragraph>(DocumentMerge.GetPreOrPostParagraphToInsert(strArray[2], index));
                        index.Parent.ReplaceChild<SimpleField>((OpenXmlElement)DocumentMerge.GetRunElementForText(strArray[0], index), index);
                    }
                    else
                        dictionary[index] = switches;
                }
            }
            foreach (KeyValuePair<SimpleField, string[]> keyValuePair in dictionary)
            {
                DocumentMerge.ExecuteSwitches((OpenXmlElement)keyValuePair.Key, keyValuePair.Value);
                keyValuePair.Key.Remove();
            }
        }

        internal static string GetColumnNameFromFieldName(string fieldname)
        {
            if (fieldname.IndexOf('_') == -1)
                return "";
            int num1 = fieldname.IndexOf('_');
            if (num1 <= 0)
                throw new ArgumentException("Error: table-MERGEFIELD should be formatted as follows: TBL_tablename_columnname.");
            int num2 = fieldname.IndexOf('_', num1 + 1);
            if (num2 <= 0)
                throw new ArgumentException("Error: table-MERGEFIELD should be formatted as follows: TBL_tablename_columnname.");
            else
                return fieldname.Substring(num2 + 1);
        }

        internal static string GetFieldName(SimpleField field, out string[] switches)
        {
            OpenXmlAttribute attribute = field.GetAttribute("instr", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            switches = new string[0];
            string str = string.Empty;
            string input = attribute.Value;
            if (!string.IsNullOrEmpty(input))
            {
                Match match = DocumentMerge.instructionRegEx.Match(input);
                if (match.Success)
                {
                    str = match.Groups["name"].ToString().Trim();
                    int length = str.IndexOf('#');
                    if (length > 0)
                    {
                        switches = str.Substring(length + 1).ToLower().Split(new char[1]
            {
              '#'
            }, StringSplitOptions.RemoveEmptyEntries);
                        str = str.Substring(0, length);
                    }
                }
            }
            return str;
        }

        internal static string GetFieldNameWithOptions(SimpleField field, out string[] switches, out string[] options)
        {
            OpenXmlAttribute attribute = field.GetAttribute("instr", "http://schemas.openxmlformats.org/wordprocessingml/2006/main");
            switches = new string[0];
            options = new string[3];
            string str = string.Empty;
            string input = attribute.Value;
            if (!string.IsNullOrEmpty(input))
            {
                Match match = DocumentMerge.instructionRegEx.Match(input);
                if (match.Success)
                {
                    str = match.Groups["name"].ToString().Trim();
                    options[0] = match.Groups["Format"].Value.Trim();
                    options[1] = match.Groups["PreText"].Value.Trim();
                    options[2] = match.Groups["PostText"].Value.Trim();
                    int length = str.IndexOf('#');
                    if (length > 0)
                    {
                        switches = str.Substring(length + 1).ToLower().Split(new char[1]
            {
              '#'
            }, StringSplitOptions.RemoveEmptyEntries);
                        str = str.Substring(0, length);
                    }
                }
            }
            return str;
        }

        internal static T GetFirstParent<T>(OpenXmlElement element) where T : OpenXmlElement
        {
            if (element.Parent == null)
                return default(T);
            if (element.Parent.GetType() == typeof(T))
                return element.Parent as T;
            else
                return DocumentMerge.GetFirstParent<T>(element.Parent);
        }

        internal static Paragraph GetPreOrPostParagraphToInsert(string text, SimpleField fieldToMimic)
        {
            Run runElementForText = DocumentMerge.GetRunElementForText(text, fieldToMimic);
            Paragraph paragraph = new Paragraph();
            paragraph.Append(new OpenXmlElement[1]
      {
        (OpenXmlElement) runElementForText
      });
            return paragraph;
        }

        internal static Run GetRunElementForText(string text, SimpleField placeHolder)
        {
            string outerXml = (string)null;
            if (placeHolder != null)
            {
                using (IEnumerator<RunProperties> enumerator = placeHolder.Descendants<RunProperties>().GetEnumerator())
                {
                    if (enumerator.MoveNext())
                        outerXml = enumerator.Current.OuterXml;
                }
            }
            Run run = new Run();
            if (!string.IsNullOrEmpty(outerXml))
                run.Append(new OpenXmlElement[1]
        {
          (OpenXmlElement) new RunProperties(outerXml)
        });
            if (!string.IsNullOrEmpty(text))
            {
                string[] strArray = ((object)text).ToString().Split(new string[1]
        {
          "\n"
        }, StringSplitOptions.None);
                bool flag1 = true;
                foreach (string str1 in strArray)
                {
                    if (!flag1)
                        run.Append(new OpenXmlElement[1]
            {
              (OpenXmlElement) new Break()
            });
                    flag1 = false;
                    bool flag2 = true;
                    string str2 = str1;
                    string[] separator = new string[1]
          {
            "\t"
          };
                    int num = 0;
                    foreach (string text1 in str2.Split(separator, (StringSplitOptions)num))
                    {
                        if (!flag2)
                            run.Append(new OpenXmlElement[1]
              {
                (OpenXmlElement) new TabChar()
              });
                        run.Append(new OpenXmlElement[1]
            {
              (OpenXmlElement) new Text(text1)
            });
                        flag2 = false;
                    }
                }
            }
            return run;
        }

        internal static Run GetRunElementForPicture(Picture picture, SimpleField placeHolder)
        {
            string outerXml = (string)null;
            if (placeHolder != null)
            {
                using (IEnumerator<RunProperties> enumerator = placeHolder.Descendants<RunProperties>().GetEnumerator())
                {
                    if (enumerator.MoveNext())
                        outerXml = enumerator.Current.OuterXml;
                }
            }
            Run run = new Run();
            if (!string.IsNullOrEmpty(outerXml))
                run.Append(new OpenXmlElement[1]
        {
          (OpenXmlElement) new RunProperties(outerXml)
        });
            if (picture == null)
                ;
            return run;
        }

        internal static string GetTableNameFromFieldName(string fieldname)
        {
            int num1 = fieldname.IndexOf('_');
            if (num1 <= 0)
                throw new ArgumentException("Error: table-MERGEFIELD should be formatted as follows: TBL_tablename_columnname.");
            int num2 = fieldname.IndexOf('_', num1 + 1);
            if (num2 <= 0)
                throw new ArgumentException("Error: table-MERGEFIELD should be formatted as follows: TBL_tablename_columnname.");
            else
                return fieldname.Substring(num1 + 1, num2 - num1 - 1);
        }

        internal static string ToTitleCase(string toConvert)
        {
            return DocumentMerge.ToTitleCaseHelper(toConvert, string.Empty);
        }

        internal static string ToTitleCaseHelper(string toConvert, string alreadyConverted)
        {
            if (string.IsNullOrEmpty(toConvert))
                return alreadyConverted;
            int num = toConvert.IndexOf(' ');
            string str;
            string toConvert1;
            if (num != -1)
            {
                str = toConvert.Substring(0, num);
                toConvert1 = toConvert.Substring(num).Trim();
            }
            else
            {
                str = toConvert.Substring(0);
                toConvert1 = string.Empty;
            }
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(alreadyConverted);
            stringBuilder.Append(" ");
            stringBuilder.Append(str.Substring(0, 1).ToUpper(CultureInfo.CurrentCulture));
            if (str.Length > 1)
                stringBuilder.Append(str.Substring(1).ToLower(CultureInfo.CurrentCulture));
            return DocumentMerge.ToTitleCaseHelper(toConvert1, ((object)stringBuilder).ToString());
        }

        internal static void ConvertFieldCodes(OpenXmlElement mainElement)
        {
            Run[] runArray = Enumerable.ToArray<Run>(mainElement.Descendants<Run>());
            Dictionary<Run, Run[]> dictionary = new Dictionary<Run, Run[]>();
            int index1 = 0;
            do
            {
                Run run1 = runArray[index1];
                int num1;
                if (run1.HasChildren && Enumerable.Count<FieldChar>(run1.Descendants<FieldChar>()) > 0)
                {
                    int num2 = (int)(FieldCharValues)Enumerable.First<FieldChar>(run1.Descendants<FieldChar>()).FieldCharType;
                    num1 = 0 != 0 ? 1 : 0;
                }
                else
                    num1 = 1;
                if (num1 == 0)
                {
                    List<Run> list = new List<Run>();
                    list.Add(run1);
                    bool flag = false;
                    string str = (string)null;
                    RunProperties runProperties = (RunProperties)null;
                    do
                    {
                        ++index1;
                        Run run2 = runArray[index1];
                        list.Add(run2);
                        if (run2.HasChildren && Enumerable.Count<FieldCode>(run2.Descendants<FieldCode>()) > 0)
                            str = str + run2.GetFirstChild<FieldCode>().Text;
                        if (run2.HasChildren && Enumerable.Count<FieldChar>(run2.Descendants<FieldChar>()) > 0 && ((FieldCharValues)Enumerable.First<FieldChar>(run2.Descendants<FieldChar>()).FieldCharType & FieldCharValues.End) == FieldCharValues.End)
                            flag = true;
                        if (run2.HasChildren && Enumerable.Count<RunProperties>(run2.Descendants<RunProperties>()) > 0)
                            runProperties = run2.GetFirstChild<RunProperties>();
                    }
                    while (!flag && index1 < runArray.Length);
                    if (!flag)
                        throw new Exception("Found a Begin FieldChar but no End !");
                    if (!string.IsNullOrEmpty(str))
                    {
                        Run key = new Run();
                        if (runProperties != null)
                            key.AppendChild<OpenXmlElement>(runProperties.CloneNode(true));
                        key.AppendChild<SimpleField>(new SimpleField()
                        {
                            Instruction = (StringValue)str
                        });
                        dictionary.Add(key, list.ToArray());
                    }
                }
                ++index1;
            }
            while (index1 < runArray.Length);
            foreach (KeyValuePair<Run, Run[]> keyValuePair in dictionary)
            {
                keyValuePair.Value[0].Parent.ReplaceChild<Run>((OpenXmlElement)keyValuePair.Key, keyValuePair.Value[0]);
                for (int index2 = 1; index2 < keyValuePair.Value.Length; ++index2)
                    keyValuePair.Value[index2].Remove();
            }
        }

        public static byte[] WordDokumanBirlestir(IList<byte[]> dokumanlar)
        {
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(dokumanlar[0], 0, dokumanlar[0].Length);
            memoryStream.Position = 0L;
            byte[] numArray;
            try
            {
                using (WordprocessingDocument wordprocessingDocument = WordprocessingDocument.Open((Stream)memoryStream, true))
                {
                    XElement xelement1 = XElement.Parse(wordprocessingDocument.MainDocumentPart.Document.Body.OuterXml);
                    for (int index = 1; index < dokumanlar.Count; ++index)
                    {
                        XElement xelement2 = XElement.Parse(WordprocessingDocument.Open((Stream)new MemoryStream(dokumanlar[index]), true).MainDocumentPart.Document.Body.OuterXml);
                        xelement1.Add((object)xelement2);
                        wordprocessingDocument.MainDocumentPart.Document.Body = new Body(((object)xelement1).ToString());
                        ((OpenXmlPartRootElement)wordprocessingDocument.MainDocumentPart.Document).Save();
                        wordprocessingDocument.Package.Flush();
                    }
                }
            }
            catch (OpenXmlPackageException ex)
            {
            }
            catch (Exception ex)
            {
            }
            finally
            {
                numArray = memoryStream.ToArray();
                memoryStream.Close();
                memoryStream.Dispose();
            }
            return numArray;
        }
    }
}
