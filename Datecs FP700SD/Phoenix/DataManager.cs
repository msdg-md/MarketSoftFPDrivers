using Phoenix.Globals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Phoenix
{
    public class DataManager
    {
        public int getAreaId()
        {
            if (Phoenix.Globals.FileSettings.IsInitialized)
            {
                SqlConnectionStringBuilder connstr = new SqlConnectionStringBuilder();
                connstr.DataSource = Globals.FileSettings.GetString(OS.Globals.DBConst.srvLocation);
                connstr.InitialCatalog = Globals.FileSettings.GetString(OS.Globals.DBConst.dbName);
                connstr.UserID = Globals.FileSettings.GetString(OS.Globals.DBConst.login);

                Log.Write($"DataManager.getAreaId. ConnectionString {connstr.ConnectionString}", Log.MessageType.Message, this);

                connstr.Password = Globals.FileSettings.GetString(OS.Globals.DBConst.psw);

                using (SqlConnection con = new SqlConnection(connstr.ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "select top 1 sareaid from LOCALCASHPROFILE";
                        try
                        {
                            con.Open();
                            int rowsAffected = (int)cmd.ExecuteScalar();
                            Log.Write($"DataManager.getAreaId executed. Value={rowsAffected} ", Log.MessageType.Message, this);
                            return rowsAffected;
                        }
                        catch (Exception ex)
                        {
                            Log.Write($"DataManager.getAreaId failed. Value=0, Error: {ex.ToString()} ", Log.MessageType.Error, this);
                        }
                        finally
                        {
                            if(con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }
                    }
                }
            }
            else
            {
                Log.Write($"DataManager.getAreaId failed. Value=0, FileSettings not initialized", Log.MessageType.Error, this);
            }
            return 0;
        }

        public void RecordZReport(int areaID)
        {
            if (AssemblySettings.ConfigurationInstance.ContainsKey(AssemblySettings.ScaleDBConnection))
            {
                string connStr = AssemblySettings.ConfigurationInstance[AssemblySettings.ScaleDBConnection];
                Log.Write($"DataManager.RecordZReport. ConnectionString {connStr}", Log.MessageType.Message, this);

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "INSERT INTO [dbo].[CLOSEDAYS]  ([SAREAID]) VALUES( @PAreaId )";
                        cmd.Parameters.AddWithValue("@PAreaId", areaID);

                        try
                        {
                            con.Open();
                            Int32 rowsAffected = cmd.ExecuteNonQuery();
                            Log.Write($"DataManager.RecordZReport executed. Rows affected {rowsAffected} ", Log.MessageType.Message, this);
                        }
                        catch (Exception ex)
                        {
                            Log.Write($"DataManager.RecordZReport failed. Error: {ex.ToString()} ", Log.MessageType.Error, this);
                        }
                        finally
                        {
                            if (con.State == ConnectionState.Open)
                            {
                                con.Close();
                            }
                        }
                    }
                }
            }
            else
            {
                Log.Write($"DataManager.RecordZReport failed. FileSettings not initialized", Log.MessageType.Error, this);
            }
        }
    }
}
