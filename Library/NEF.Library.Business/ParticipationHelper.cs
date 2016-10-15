using NEF.Library.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace NEF.Library.Business
{
    public static class ParticipationHelper
    {
        public static MsCrmResultObject GetParticipations(SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                P.new_sourceofparticipationId ParticipationId
	                                ,P.new_name Name
                                FROM
	                                new_sourceofparticipation P WITH (NOLOCK)
                                WHERE
	                                P.StateCode = 0";
                #endregion
                DataTable dt = sda.getDataTable(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET PARTICIPATIONS |
                    returnValue.ReturnObject = dt.ToList<Participation>();
                    #endregion

                    returnValue.Success = true;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Sistemde etkin katılım kaynağı bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetSubParticipations(Guid participationId, SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                P.new_subsourceofparticipationId SubParticipationId
	                                ,P.new_name Name
                                FROM
	                                new_subsourceofparticipation P WITH (NOLOCK)
                                WHERE
	                                P.new_participationsourceid = '{0}'
	                                AND
	                                P.StateCode = 0";
                #endregion
                DataTable dt = sda.getDataTable(string.Format(query, participationId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET SUB PARTICIPATIONS |
                    returnValue.ReturnObject = dt.ToList<SubParticipation>();
                    #endregion

                    returnValue.Success = true;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Seçilen katılım kaynağına ait alt katılım kaynağı bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }

        public static MsCrmResultObject GetChannels(SqlDataAccess sda)
        {
            MsCrmResultObject returnValue = new MsCrmResultObject();
            try
            {
                #region | SQL QUERY |
                string query = @"SELECT
	                                P.new_channelofawarenessId ChannelId
	                                ,P.new_name Name
                                FROM
	                                new_channelofawareness P WITH (NOLOCK)
                                WHERE
	                                P.StateCode = 0
                                ORDER BY
	                                P.new_name ASC";
                #endregion
                DataTable dt = sda.getDataTable(query);

                if (dt != null && dt.Rows.Count > 0)
                {
                    #region | GET PARTICIPATIONS |
                    returnValue.ReturnObject = dt.ToList<Channel>();
                    #endregion

                    returnValue.Success = true;
                }
                else
                {
                    returnValue.Success = false;
                    returnValue.Result = "Sistemde etkin haberdar olma kaynağı bulunmamaktadır!";
                }
            }
            catch (Exception ex)
            {
                returnValue.Success = false;
                returnValue.Result = ex.Message;
            }

            return returnValue;
        }
    }
}
