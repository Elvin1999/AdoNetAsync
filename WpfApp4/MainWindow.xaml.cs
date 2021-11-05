using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
namespace WpfApp4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var conn=new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
                await conn.OpenAsync();
                SqlCommand command = conn.CreateCommand();
                command.CommandText = "WAITFOR DELAY '00:00:05';";
                command.CommandText += txtbox.Text;

                var table = new DataTable();
                using (var reader=await command.ExecuteReaderAsync())
                {
                    do
                    {
                        bool hasColumnAdded = false;
                        while (await reader.ReadAsync())
                        {

                        if (!hasColumnAdded)
                        {
                            hasColumnAdded = true;
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                table.Columns.Add(reader.GetName(i));
                            }
                        }

                        var row = table.NewRow();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[i] = await reader.GetFieldValueAsync<Object>(i);
                        }
                        table.Rows.Add(row);
                        }

                    } while (reader.NextResult());

                    datagrid.ItemsSource = table.DefaultView;
                }


            }

        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using (var conn=new SqlConnection())
            {
                conn.ConnectionString = ConfigurationManager.ConnectionStrings["MyConnString"].ConnectionString;
                await conn.OpenAsync();
                var command = conn.CreateCommand();
                command.CommandText = "sp_UpdateBook";
                command.CommandType = CommandType.StoredProcedure;

                var p1 = new SqlParameter();
                p1.Value = int.Parse(txtbox.Text);
                p1.SqlDbType = SqlDbType.Int;
                p1.ParameterName = "@MyId";

                var p2 = new SqlParameter();
                p2.Value = 9999;
                p2.SqlDbType = SqlDbType.Int;
                p2.ParameterName = "@Page";

                command.Parameters.Add(p1);
                command.Parameters.Add(p2);

                await command.ExecuteNonQueryAsync();
                MessageBox.Show("Updated Successfully");

            }
        }
    }
}
