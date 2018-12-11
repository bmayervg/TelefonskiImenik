using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelefonskiImenik;

namespace TelefonskiImenik.Business
{
    class cTelefonPretraga
    {
        public static DataTable SearchTelefonskiImenik(string grad, string postanskiBroj, string predBroj, string TelefonskiBroj, string ulica, string ime, string prezime, bool exported, int pageSize, int currentPageIndex, ref int TotalPageCount )
        {
            DataTable tblSearchResults = new DataTable();
            SqlConnection conn = new SqlConnection(TelefonskiImenik.Program._connectionString);
            SqlCommand cmd = new SqlCommand("spTelefonskiImenik_Search", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            if (!string.IsNullOrEmpty(grad))
            {
                cmd.Parameters.AddWithValue("@gradNaziv", grad);
            }
            if (!string.IsNullOrEmpty(postanskiBroj))
            {
                cmd.Parameters.AddWithValue("@postanskiBroj", postanskiBroj);
            }
            if (!string.IsNullOrEmpty(predBroj))
            {
                cmd.Parameters.AddWithValue("@predBroj", predBroj);
            }
            if (!string.IsNullOrEmpty(TelefonskiBroj))
            {
                cmd.Parameters.AddWithValue("@TelefonskiBroj", TelefonskiBroj);
            }
            if (!string.IsNullOrEmpty(ulica))
            {
                cmd.Parameters.AddWithValue("@ulica", ulica);
            }
            if (!string.IsNullOrEmpty(ime))
            {
                cmd.Parameters.AddWithValue("@ime", ime);
            }
            if (!string.IsNullOrEmpty(prezime))
            {
                cmd.Parameters.AddWithValue("@prezime", prezime);
            }
            cmd.Parameters.AddWithValue("@exported", exported);
            cmd.Parameters.AddWithValue("@pageSize", pageSize);
            cmd.Parameters.AddWithValue("@currentPageIndex", currentPageIndex);
            SqlParameter p = new SqlParameter();
            p.ParameterName = "@TotalPageCount";
            p.Direction = ParameterDirection.Output;
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(tblSearchResults);
            return tblSearchResults;
        }
    }
}
