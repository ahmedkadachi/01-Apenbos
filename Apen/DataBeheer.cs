using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace Apen
{
    class DataBeheer
    {
        private string connectionString;
        public DataBeheer(string connectionString)
        {
            this.connectionString = connectionString;
        }
        private SqlConnection getConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            return connection;
        }

        public async Task VoegBoomToe(List<Boom> boom, Bos bos)
        {
            SqlConnection connection = getConnection();
            string query = "INSERT INTO dbo.WoodRecords (woodID, treeID, x, y) VALUES(@bosID, @ID, @X, @Y)";
            foreach (Boom element in boom)
            {
                 using(SqlCommand command = connection.CreateCommand())
                {
                    await Task.Run(()=>connection.Open());
                    try
                    {
                        Console.WriteLine(" START write to WoodRecords ");
                        command.Parameters.AddWithValue("@bosID", SqlDbType.Int).Value = bos.ID;
                        command.Parameters.AddWithValue("@ID", SqlDbType.Int).Value = element.ID;
                        command.Parameters.AddWithValue("@X", SqlDbType.Int).Value = element.X;
                        command.Parameters.AddWithValue("@Y", SqlDbType.Int).Value = element.Y;
                        command.CommandText = query;
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                    finally
                    {
                        connection.Close();
                    }
                    Console.WriteLine(" STOP write to WoodRecords ");
                }
            }
        }
        public async Task VerwijderAlleBomen()
        {
            SqlConnection connection = getConnection();
            string query = "DELETE FROM dbo.WoodRecords";
            await using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                try
                {
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task VoegAapGegevens(List<Aap> apen, List<Boom> bomen, Bos bos)
        {
            SqlConnection connection = getConnection();
            string query = "INSERT INTO dbo.MonkeyRecords (monkeyID, monkeyName, woodID, seqnr, treeID, x,y) VALUES(@aapID, @aapNaam, @bosID, @seqnr, @boomID, @X, @Y)";
            foreach (Aap element in apen)
            {
                int seqnr = 0;
                foreach (Boom el in element.Boom_Lijst)
                {
                    using (SqlCommand command = connection.CreateCommand())
                    {
                        await Task.Run(()=>connection.Open());
                        try
                        {
                            Console.WriteLine(" START write to MonkeyRecords ");
                            command.Parameters.AddWithValue("@aapID", SqlDbType.Int).Value = element.ID;
                            command.Parameters.AddWithValue("@aapNaam", SqlDbType.Int).Value = element.Naam;
                            command.Parameters.AddWithValue("@bosID", SqlDbType.Int).Value = bos.ID;
                            command.Parameters.AddWithValue("@seqnr", SqlDbType.Int).Value = seqnr;
                            command.Parameters.AddWithValue("@boomID", SqlDbType.Int).Value = el.ID;
                            command.Parameters.AddWithValue("@X", SqlDbType.Int).Value = el.X;
                            command.Parameters.AddWithValue("@Y", SqlDbType.Int).Value = el.Y;
                            command.CommandText = query;
                            command.ExecuteNonQuery();
                            seqnr++;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        finally
                        {
                            connection.Close();
                        }
                        Console.WriteLine(" STOP write to MonkeyRecords ");
                    }
                }
            }
        }
        public async Task VerwijderAlleApenGegevens()
        {
            SqlConnection connection = getConnection();
            string query = "DELETE FROM dbo.MonkeyRecords";
            await using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                try
                {
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public async Task VoegLogGegevens(List<Aap> apen, Bos bos)
        {
            SqlConnection connection = getConnection();
            string query = "INSERT INTO dbo.Logs (woodID, monkeyID, message) VALUES(@bosID, @aapID, @message)";
            foreach (Aap element in apen)
            {
                int seqnr = 0;
                foreach (Boom el in element.Boom_Lijst)
                {
                     using (SqlCommand command = connection.CreateCommand())
                    {
                        await Task.Run(()=> connection.Open());
                        try
                        {
                            Console.WriteLine(" START write to LOGS ");
                            command.Parameters.AddWithValue("@aapID", SqlDbType.Int).Value = element.ID;
                            command.Parameters.AddWithValue("@bosID", SqlDbType.Int).Value = bos.ID;
                            if(el.Y == -1)
                                command.Parameters.AddWithValue("@message", SqlDbType.NChar).Value = element.Naam + " is out the woods";
                            else
                                command.Parameters.AddWithValue("@message", SqlDbType.NChar).Value = element.Naam+" is now in tree "+el.ID+" at location ("+el.X+","+el.Y+")";

                            command.CommandText = query;
                            command.ExecuteNonQuery();
                            seqnr++;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                        finally
                        {
                            connection.Close();
                        }
                        Console.WriteLine(" STOP write to LOGS ");
                    }
                }
            }
        }
        public async Task VerwijderAlleLogGegevens()
        {
            SqlConnection connection = getConnection();
            string query = "DELETE FROM dbo.Logs";
            await using (SqlCommand command = connection.CreateCommand())
            {
                connection.Open();
                try
                {
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    connection.Close();
                }
            }
        }
    }
}
