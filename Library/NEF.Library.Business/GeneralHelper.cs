using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NEF.Library.Business
{
    public static class GeneralHelper
    {
        /*Converts DataTable To List*/
        //List<Student> newStudents = studentTbl.ToList<Student>(); 
        public static List<TSource> ToList<TSource>(this DataTable dataTable) where TSource : new()
        {
            var dataList = new List<TSource>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;
            var objFieldNames = (from PropertyInfo aProp in typeof(TSource).GetProperties(flags)
                                 select new
                                 {
                                     Name = aProp.Name,
                                     Type = Nullable.GetUnderlyingType(aProp.PropertyType) ?? aProp.PropertyType
                                 }).ToList();
            var dataTblFieldNames = (from DataColumn aHeader in dataTable.Columns
                                     select new { Name = aHeader.ColumnName, Type = aHeader.DataType }).ToList();
            var commonFields = objFieldNames.Intersect(dataTblFieldNames).ToList();

            foreach (DataRow dataRow in dataTable.AsEnumerable().ToList())
            {
                var aTSource = new TSource();
                foreach (var aField in commonFields)
                {
                    if (dataRow[aField.Name] != DBNull.Value)
                    {
                        PropertyInfo propertyInfos = aTSource.GetType().GetProperty(aField.Name);
                        propertyInfos.SetValue(aTSource, dataRow[aField.Name], null);
                    }
                }
                dataList.Add(aTSource);
            }
            return dataList;
        }

        public static MsCrmResultObject GetOptionSetValues(int objectTypeCode, string attributeName, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                SM.Value Name
	                                ,SM.AttributeValue Value
                                FROM
	                                StringMap SM WITH (NOLOCK)
                                WHERE
	                                SM.ObjectTypeCode = {0}
	                                AND
	                                SM.AttributeName = '{1}'
                                ORDER BY
                                     SM.Value ASC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, objectTypeCode, attributeName));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET VALUES |
                    List<StringMap> returnList = new List<StringMap>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        StringMap _value = new StringMap();
                        _value.Value = (int)dt.Rows[i]["Value"];
                        _value.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_value);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Böyle bir alan bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetStatusCodesByState(int objectTypeCode, int stateCode, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                a.Value Name
	                                ,a.AttributeValue Value
                                FROM 
	                                StatusMap SM WITH (NOLOCK)
                                INNER JOIN
	                                StringMap a WITH (NOLOCK)
	                                ON
	                                a.ObjectTypeCode = {0}
	                                and
	                                a.AttributeName = 'statuscode'
	                                and
	                                a.AttributeValue = SM.Status
                                WHERE
	                                SM.ObjectTypeCode = {0}
	                                AND
	                                SM.State = {1}
                                ORDER BY
                                     a.Value ASC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, objectTypeCode, stateCode));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET VALUES |
                    List<StringMap> returnList = new List<StringMap>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        StringMap _value = new StringMap();
                        _value.Value = (int)dt.Rows[i]["Value"];
                        _value.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_value);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Böyle bir alan bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetProductActiveStateCodes(SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                SM.Value AS Name
	                                ,SM.AttributeValue AS Value
                                FROM
	                                StringMap SM WITH (NOLOCK)
                                WHERE
	                                SM.ObjectTypeCode = 1024
	                                AND
	                                SM.AttributeName = 'statecode'";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET VALUES |
                    List<StringMap> returnList = new List<StringMap>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        StringMap _value = new StringMap();
                        _value.Value = (int)dt.Rows[i]["Value"];
                        _value.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_value);
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Böyle bir alan bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetProductActiveStatusCodes(int state, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                    sm.Value AS Name
	                                    ,sm.AttributeValue AS Value
                                    FROM
	                                    StatusMap AS stm (NOLOCK)
                                    JOIN
	                                    StringMap AS sm (NOLOCK)
		                                    ON
			                                    sm.AttributeValue=stm.Status
			                                    AND
			                                    sm.ObjectTypeCode=stm.ObjectTypeCode
			                                    AND
			                                    sm.AttributeName='statuscode'
                                    WHERE
	                                    stm.ObjectTypeCode=1024
                                    AND
	                                    stm.State={0}
									ORDER BY
										sm.DisplayOrder";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, state));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET VALUES |
                    List<StringMap> returnList = new List<StringMap>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        StringMap _value = new StringMap();
                        _value.Value = (int)dt.Rows[i]["Value"];
                        _value.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_value);

                        if (i == 0) // 2. sırada gösterebilmek için
                        {
                            returnList.Add(new StringMap() { Name = "Hepsi", Value = null });
                        }
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Böyle bir alan bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetRentalProductActiveStatusCodes(SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                    stm.Value AS Name
	                                    ,stm.AttributeValue AS Value
                                    FROM
	                                    StringMap AS stm (NOLOCK)
                                    WHERE
			                                    stm.AttributeName='new_usedrentalandsalesstatus'
                                  
									ORDER BY
										stm.DisplayOrder";
                #endregion

                DataTable dt = sda.getDataTable(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET VALUES |
                    List<StringMap> returnList = new List<StringMap>();

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (i == 0) // 2. sırada gösterebilmek için
                        {
                            returnList.Add(new StringMap() { Name = "Hepsi", Value = null });
                        }

                        StringMap _value = new StringMap();
                        _value.Value = (int)dt.Rows[i]["Value"];
                        _value.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_value);

                       
                    }
                    #endregion

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Böyle bir alan bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static string ToTurkishCharacter(this string word)
        {
            word = word.Replace('ö', 'o');
            word = word.Replace('ü', 'u');
            word = word.Replace('ğ', 'g');
            word = word.Replace('ş', 's');
            word = word.Replace('ı', 'i');
            word = word.Replace('ç', 'c');
            return word;
        }

        /// <summary>
        /// Mail gönderme işlemini yapar
        /// </summary>
        /// <param name="ObjectId">Varlık ObjectId</param>
        /// <param name="ObjectType">Varlık ObjectType</param>
        /// <param name="fromParty">Gönderen bilgisi</param>
        /// <param name="toParty">Gönderilecek bilgisi</param>
        /// <param name="subject">Konu</param>
        /// <param name="mailBody">Metin</param>
        /// <param name="service">Crm Servis</param>
        /// <returns>MsCrmResult Class döner</returns>
        public static MsCrmResult SendMail(Guid ObjectId, string ObjectType, Entity[] fromParty, Entity[] toParty, string subject, string mailBody, Annotation _anno, IOrganizationService service)
        {

            MsCrmResult returnValue = new MsCrmResult();
            returnValue.CrmId = Guid.Empty;
            try
            {
                #region Create Email

                Entity email = new Entity("email");
                email["to"] = toParty;
                email["from"] = fromParty;
                email["subject"] = subject;
                email["description"] = mailBody;
                email["directioncode"] = true;

                if (ObjectId != Guid.Empty && !string.IsNullOrEmpty(ObjectType))
                {
                    EntityReference regardingObject = new EntityReference(ObjectType, ObjectId);
                    email.Attributes.Add("regardingobjectid", regardingObject);
                }


                returnValue.CrmId = service.Create(email);


                #endregion

                #region Create Attachment
                if (_anno != null)
                {
                    Entity attachment = new Entity("activitymimeattachment");
                    attachment["subject"] = _anno.Subject;
                    attachment["filename"] = _anno.Subject;
                    attachment["mimetype"] = _anno.MimeType;
                    attachment["body"] = _anno.DocumentBody;
                    attachment["attachmentnumber"] = 1;
                    attachment["objectid"] = new EntityReference("email", returnValue.CrmId);
                    attachment["objecttypecode"] = "email";
                    service.Create(attachment);
                }
                #endregion

                #region Send Email
                var req = new SendEmailRequest
                {
                    EmailId = returnValue.CrmId,
                    TrackingToken = string.Empty,
                    IssueSend = true
                };
                var res = (SendEmailResponse)service.Execute(req);
                returnValue.Success = true;
                #endregion
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = "SendMail|" + ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetQueues(QueueTypes type, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                Q.QueueId
	                                ,Q.Name
                                FROM
	                                Queue Q WITH (NOLOCK)
                                WHERE
	                                Q.StateCode = 0
	                                AND
	                                Q.new_queuetype = {0}";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, (int)type));
                if (dt != null && dt.Rows.Count > 0)
                {
                    List<Queue> returnList = new List<Queue>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Queue _queue = new Queue();
                        _queue.QueueId = (Guid)dt.Rows[i]["QueueId"];
                        _queue.Name = dt.Rows[i]["Name"] != DBNull.Value ? dt.Rows[i]["Name"].ToString() : string.Empty;

                        returnList.Add(_queue);
                    }

                    returnValue.Success = true;
                    returnValue.ReturnObject = returnList;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static MsCrmResult GetEntityIdByAttributeName(string entityName, string attributeName, string value, SqlDataAccess sda)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                #region | SQL QUERY |

                string sqlQuery = @"SELECT
	                                e.{0}Id AS Id
                                FROM
	                                {0} AS e (NOLOCK)
                                WHERE
	                                e.{1}='{2}'";

                #endregion

                DataTable dt = sda.getDataTable(string.Format(sqlQuery, entityName, attributeName, value));

                if (dt.Rows.Count > 0)
                {
                    returnValue.Success = true;
                    returnValue.Result = "Kayıt içeride tanımlıdır";

                    returnValue.CrmId = (Guid)dt.Rows[0]["Id"];
                }
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResult CreateEntity(string EntityName, string AttributeName, string AttributeValue, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                Entity ent = new Entity(EntityName);
                ent[AttributeName] = AttributeValue;

                returnValue.CrmId = service.Create(ent);
                returnValue.Success = true;
                returnValue.Result = "Kayıt oluşturuldu.";
            }
            catch (Exception ex)
            {
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        /// <summary>
        /// Email numarasının formatını kontrol eder
        /// </summary>
        /// <param name="email">Email adresi</param>
        /// <returns>bool döner</returns>
        public static bool CheckEmail(string email)
        {
            string strPattern = @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
            if (System.Text.RegularExpressions.Regex.IsMatch(email, strPattern))
            { return true; }

            return false;
        }

        /// <summary>
        /// Telefon numarasının formatını kontrol eder
        /// </summary>
        /// <param name="phoneNo">Telefon numarası</param>
        /// <returns>TelephoneNumber Class döner</returns>
        public static TelephoneNumber CheckTelephoneNumber(string phoneNo)
        {
            string countryCode = string.Empty;
            string phoneCode = string.Empty;
            string phoneNumber = string.Empty;

            if (phoneNo.Length == 15)
            {
                countryCode = phoneNo.Substring(1, 2);
                phoneCode = phoneNo.Substring(4, 3);
                phoneNumber = phoneNo.Substring(8, 7);

                TelephoneNumber tn = new TelephoneNumber();
                tn.TelephoneNo = phoneNo;
                tn.countryCode = countryCode;
                tn.phoneCode = phoneCode;
                tn.phoneNo = phoneNumber;

                if (IsPhoneNumberCorrect(phoneNumber))
                    tn.isFormatOK = true;
                else
                    tn.isFormatOK = false;

                return tn;
            }
            else
            {
                return new TelephoneNumber()
                {
                    countryCode = countryCode,
                    phoneCode = phoneCode,
                    phoneNo = phoneNumber,
                    isFormatOK = false
                };
            }
        }

        /// <summary>
        /// Telefon numarasında tekrarlayan rakamları kontrol eder
        /// </summary>
        /// <param name="phoneNo">Telefon numarası</param>
        /// <returns>bool döner</returns>
        public static bool IsPhoneNumberCorrect(string phoneNo)
        {
            bool returnValue = true;

            string[] wrongForms = new string[] { "1111111", "2222222", "3333333", "4444444", "5555555", "6666666", "7777777", "8888888", "9999999", "0000000", "1234567", "2345678", "3456789", "9876543", "0123456", "7654321", "8765432" };

            if (phoneNo.Length != 7)
            {
                returnValue = false;
            }
            else
            {
                for (int i = 0; i < wrongForms.Length; i++)
                {
                    if (wrongForms[i] == phoneNo)
                        returnValue = false;
                }
            }
            return returnValue;
        }

        public static MsCrmResult CheckIdentityNumber(string identityNumber)
        {
            MsCrmResult returnValue = new MsCrmResult();
            returnValue.Success = true;
            if (identityNumber.Length != 11)
            {
                returnValue.Success = false;
                returnValue.Result = "Müşterinin Tc Kimlik Numarası eksik girildi!";
                return returnValue;
            }

            int pr1 = int.Parse(identityNumber.Substring(0, 1));
            int pr2 = int.Parse(identityNumber.Substring(1, 1));
            int pr3 = int.Parse(identityNumber.Substring(2, 1));
            int pr4 = int.Parse(identityNumber.Substring(3, 1));
            int pr5 = int.Parse(identityNumber.Substring(4, 1));
            int pr6 = int.Parse(identityNumber.Substring(5, 1));
            int pr7 = int.Parse(identityNumber.Substring(6, 1));
            int pr8 = int.Parse(identityNumber.Substring(7, 1));
            int pr9 = int.Parse(identityNumber.Substring(8, 1));
            int pr10 = int.Parse(identityNumber.Substring(9, 1));
            int pr11 = int.Parse(identityNumber.Substring(10, 1));
            if ((pr1 + pr3 + pr5 + pr7 + pr9 + pr2 + pr4 + pr6 + pr8 + pr10) % 10 != pr11)
            {
                returnValue.Success = false;
                returnValue.Result = "Müşterinin TC Kimlik Numarası hatalı!";
                return returnValue;
            }
            if (((pr1 + pr3 + pr5 + pr7 + pr9) * 7 + (pr2 + pr4 + pr6 + pr8) * 9) % 10 != pr10)
            {
                returnValue.Success = false;
                returnValue.Result = "Müşterinin TC Kimlik Numarası hatalı!";
                return returnValue;
            }
            if (((pr1 + pr3 + pr5 + pr7 + pr9) * 8) % 10 != pr11)
            {
                returnValue.Success = false;
                returnValue.Result = "Müşterinin TC Kimlik Numarası hatalı!";
                return returnValue;
            }
            return returnValue;
        }

        public static MsCrmResult AssociateEntityToEntity(Guid entityId, string entityName, EntityReference entity, string relationShipName, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();
            try
            {
                #region | ASSOCIATE PROCESS |
                AssociateEntitiesRequest request = new AssociateEntitiesRequest();
                request.Moniker1 = new EntityReference(entityName, entityId);
                request.Moniker2 = entity;
                request.RelationshipName = relationShipName;
                service.Execute(request);
                #endregion

                returnValue.Success = true;
                returnValue.Result = "İlişkilendirme tamamlandı.";
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static void WriteToText(string text, string file)
        {
            FileStream fs = null;
            if (!File.Exists(file))
            {
                using (fs = File.Create(file))
                {

                }
            }

            if (File.Exists(file))
            {
                using (StreamWriter sw = new StreamWriter(file, true))
                {

                    sw.WriteLine(text);
                    sw.Close();
                }
            }
        }

        public static MsCrmResultObject GetAttachmentByObjectId(Guid objectId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                TOP 1
	                                A.Subject
	                                ,A.MimeType
	                                ,A.DocumentBody
                                FROM
	                                Annotation A WITH (NOLOCK)
                                WHERE
	                                A.ObjectId = '{0}'
                                ORDER BY
	                                A.CreatedOn DESC";
                #endregion

                DataTable dt = sda.getDataTable(string.Format(query, objectId));
                if (dt != null && dt.Rows.Count > 0)
                {
                    Annotation attachment = new Annotation();
                    attachment.Subject = dt.Rows[0]["Subject"] != DBNull.Value ? dt.Rows[0]["Subject"].ToString() : string.Empty;
                    attachment.MimeType = dt.Rows[0]["MimeType"] != DBNull.Value ? dt.Rows[0]["MimeType"].ToString() : string.Empty;

                    //byte[] array = (byte[])dt.Rows[0]["DocumentBody"];
                    //attachment.DocumentBody = Convert.ToBase64String(array);

                    attachment.DocumentBody = dt.Rows[0]["DocumentBody"].ToString();

                    returnValue.Success = true;
                    returnValue.ReturnObject = attachment;
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }
            return returnValue;
        }

        public static string ToTitleCase(string Text)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Text);
        }

        public static MsCrmResult CreateTypeOfHome(string name, Guid generalHomeTypeId, IOrganizationService service)
        {
            MsCrmResult returnValue = new MsCrmResult();

            try
            {
                Entity ent = new Entity("new_typeofhome");
                ent["new_name"] = name;
                ent["new_generaltypeofhomeid"] = new EntityReference("new_generaltypeofhome", generalHomeTypeId);

                returnValue.CrmId = service.Create(ent);

                returnValue.Success = true;
            }
            catch (Exception ex)
            {

                returnValue.Result = ex.Message;
            }

            return returnValue;
        }
    }
}
