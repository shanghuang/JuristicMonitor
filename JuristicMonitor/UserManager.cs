using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data;
using MySql.Data.MySqlClient;

namespace JuristicMonitor
{
    class UserManager
    {
        String user_table_name = "userdata";
        String userstock_table_name = "userstock";

        public void SetupDatebase(MySqlConnection conn)
        {
            try
            {
                string sql_create_db = "CREATE TABLE IF NOT EXISTS " + user_table_name + " (" +
                                        "idx INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                                        "username   VARCHAR(128), " +
                                        "password   VARCHAR(128), " +
                                        "email      VARCHAR(128) ) ";

                MySqlCommand cmd_create = new MySqlCommand(sql_create_db, conn);
                cmd_create.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show("Error " + ex.Number + " has occurred: " + ex.Message,
                //    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                string sql_create_db = "CREATE TABLE IF NOT EXISTS " + userstock_table_name + " (" +
                                        "idx INT UNSIGNED NOT NULL AUTO_INCREMENT PRIMARY KEY, " +
                                        "username   VARCHAR(128), " +
                                        "stock_index   VARCHAR(12) ) ";

                MySqlCommand cmd_create = new MySqlCommand(sql_create_db, conn);
                cmd_create.ExecuteNonQuery();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show("Error " + ex.Number + " has occurred: " + ex.Message,
                //    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
