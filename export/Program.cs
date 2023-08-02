using System.Data;
using System.Data.SqlClient;

class Program
{
    static string ConnectionString = "Data Source=DESKTOP-V99B027;Initial Catalog=master;Integrated Security=True";
    //Generating File from Byte Array.
    public static bool ByteArrayToFile(string FullFilePath, byte[] byteArray)
    {
        try
        {
            using (var fs = new FileStream(FullFilePath, FileMode.Create, FileAccess.Write))
            {
                fs.Write(byteArray, 0, byteArray.Length);
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception caught"+ex.Message);
            return false;
        }
    }
    //Exporting all Data of DataColumnName Column and appending with ColumnName1+DataColumnName+ColumnName2 as fileName.
    public static void ExportData(string FolderPath,string ColumnName1,string DataColumnName,string ColumnName2,string TableName)
    {
        Console.WriteLine("Exporting File");
        string query = "SELECT " + ColumnName1 + "," + ColumnName2 + "," + DataColumnName + " FROM " + TableName;
        Console.WriteLine("Query: "+query);
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            using (var sqlQuery = new SqlCommand(query, connection))
            {
                connection.Open();
                SqlDataReader reader = sqlQuery.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var var1 = reader.GetValue(0).ToString();
                        var var2 = reader.GetValue(1).ToString();
                        string fullPath = FolderPath + var1 + var2;
                        byte[] var3 = (byte[])reader[2];
                        if (ByteArrayToFile(fullPath, var3))
                        {
                            Console.WriteLine("Exported: "+fullPath);
                        }
                        else
                        {
                            Console.WriteLine(var2 + ": Failed to Write File");
                        }
                    }
                    connection.Close();
                }
            }
        }
    }
    public static Boolean InsertData(int id,string FullFilePath,string FileName)
    {
        using (SqlConnection Conection = new SqlConnection(ConnectionString))
        {
            Conection.Open();
            using (SqlCommand Command = new SqlCommand("INSERT INTO FileTable VALUES(@Id,@doc,@FileName);", Conection))
            {
                byte[] bytes = File.ReadAllBytes(FullFilePath);
                Command.CommandType = CommandType.Text;
                Command.Parameters.Add("@id", SqlDbType.Int, 10).Value = id;
                Command.Parameters.Add("@doc", SqlDbType.Binary, 800000000).Value = bytes;
                Command.Parameters.Add("@FileName", SqlDbType.VarChar, 2000).Value = FileName;
                Command.ExecuteNonQuery();
            }
            Conection.Close();
        }
        return true;
    }
    public static void Main(string[] args)
    {
        //InsertData(1, "C:\\Users\\saket\\OneDrive\\Documents\\pdfSample.pdf", "pdfSample.pdf");
        ExportData("C:\\ExportDataDB\\", "id", "Doc", "file_name","FileTable");
    }

}
