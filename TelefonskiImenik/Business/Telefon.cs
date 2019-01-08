using System;
using System.Data;
using System.Data.SqlClient;

namespace TelefonskiImenik.Business
{
    public class cTelefon
    {
        public static void SnimiUpitOdgovorRegistarNeZovi(int id_Telefon, string postData, string httpResponse, int id_UpitTip)
        {
            SqlConnection conn = new SqlConnection(TelefonskiImenik.Program._connectionString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("spTelefonPostResponse_Insert", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_Telefon", id_Telefon);
                cmd.Parameters.AddWithValue("@id_UpitTip", id_UpitTip);
                cmd.Parameters.AddWithValue("@postData", postData);
                cmd.Parameters.AddWithValue("@httpResponse", httpResponse);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed && conn.State != ConnectionState.Broken)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch
                    { }

                }
                conn = null;
            }

        }

        public static void UpdateTelefonExported(int id_Telefon, string exportFileName)
        {
            SqlConnection conn = new SqlConnection(TelefonskiImenik.Program._connectionString);
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("spTelefon_UpdateExported", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id_Telefon", id_Telefon);
                cmd.Parameters.AddWithValue("@exportFileName", exportFileName);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State != ConnectionState.Closed && conn.State != ConnectionState.Broken)
                {
                    try
                    {
                        conn.Close();
                    }
                    catch
                    { }

                }
                conn = null;
            }
        }
    }
}
