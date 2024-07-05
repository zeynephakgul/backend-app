using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using backend_app.Models;

namespace backend_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CaseStudyController : ControllerBase
    {

        private IConfiguration _configuration;

        public CaseStudyController( IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //User Controller Methods

        [HttpGet]
        [Route("GetUsers")]

        public JsonResult GetUsers()
        {
            string query = "select * from dbo.Users";
            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString( "CaseStudyDBCon");
            SqlDataReader myReader;
            
            using( SqlConnection myCon = new SqlConnection( sqlDataSource))
            {
                myCon.Open();
                using( SqlCommand myCommand = new SqlCommand( query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }

            return new JsonResult(table);
        }

        [HttpGet]
        [Route("GetUserById")]
        public IActionResult GetUserById(int userId)
        {
            string sqlDataSource = _configuration.GetConnectionString("CaseStudyDBCon");
            string query = @"SELECT UserId, FullName, Email FROM dbo.Users WHERE UserId = @UserId";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", userId);

                    myCon.Open();
                    SqlDataReader myReader = myCommand.ExecuteReader();

                    if (myReader.Read())
                    {
                        var user = new
                        {
                            UserId = myReader.GetInt32(0),
                            FullName = myReader.GetString(1),
                            Email = myReader.GetString(2)
                        };

                        myReader.Close();
                        return Ok(user);
                    }
                    else
                    {
                        myReader.Close();
                        return NotFound("User not found");
                    }
                }
            }
        }

        [HttpPut("UpdateUser")]
        public IActionResult UpdateUser(int userId, string fullName, string email)
        {
            if (userId <= 0)
            {
                return BadRequest("Invalid user ID");
            }

            string sqlDataSource = _configuration.GetConnectionString("CaseStudyDBCon");
            string query = @"UPDATE dbo.Users 
                     SET FullName = @FullName, Email = @Email, UpdatedAt = GETDATE() 
                     WHERE UserId = @UserId";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", userId);
                    myCommand.Parameters.AddWithValue("@FullName", fullName);
                    myCommand.Parameters.AddWithValue("@Email", email);

                    myCon.Open();
                    int result = myCommand.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return Ok("User updated successfully");
                    }
                    else
                    {
                        return BadRequest("Failed to update user");
                    }
                }
            }
        }

        //DELETE
        [HttpDelete("DeleteUser")]
        public IActionResult DeleteUser(int userId)
        {
            string sqlDataSource = _configuration.GetConnectionString("CaseStudyDBCon");
            string query = @"DELETE FROM dbo.Users WHERE UserId = @UserId";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", userId);

                    myCon.Open();
                    int result = myCommand.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return Ok("User deleted successfully");
                    }
                    else
                    {
                        return BadRequest("Failed to delete user");
                    }
                }
            }
        }




        [HttpGet]
        [Route("GetUserIdByUsername")]
        public IActionResult GetUserIdByUsername(string username)
        {
            string sqlDataSource = _configuration.GetConnectionString("CaseStudyDBCon");
            string query = @"SELECT UserId FROM dbo.Users WHERE Username = @Username";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Username", username);

                    myCon.Open();
                    SqlDataReader myReader = myCommand.ExecuteReader();

                    if (myReader.Read())
                    {
                        int userId = myReader.GetInt32(0);
                        myReader.Close();
                        return Ok(userId);
                    }
                    else
                    {
                        myReader.Close();
                        return NotFound("User not found");
                    }
                }
            }
        }

        [HttpGet]
        [Route("GetUserNameById")]
        public IActionResult GetUserNameById(int userId)
        {
            string sqlDataSource = _configuration.GetConnectionString("CaseStudyDBCon");
            string query = @"SELECT FullName FROM dbo.Users WHERE UserId = @UserId";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", userId);

                    myCon.Open();
                    SqlDataReader myReader = myCommand.ExecuteReader();

                    if (myReader.Read())
                    {
                        string fullName = myReader.GetString(0);
                        myReader.Close();
                        return Ok(fullName);
                    }
                    else
                    {
                        myReader.Close();
                        return NotFound("User not found");
                    }
                }
            }
        }

        [HttpPost]
        [Route("AddUser")]
        public IActionResult AddUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest("User object is null");
            }

            string sqlDataSource = _configuration.GetConnectionString("CaseStudyDBCon");
            string query = @"INSERT INTO dbo.Users (Username, PasswordHash, Email, FullName, CreatedAt, UpdatedAt)
            VALUES (@Username, @PasswordHash, @Email, @FullName, GETDATE(), GETDATE())";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Username", user.Username);
                    myCommand.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                    myCommand.Parameters.AddWithValue("@Email", user.Email);
                    myCommand.Parameters.AddWithValue("@FullName", user.FullName);

                    myCon.Open();
                    int result = myCommand.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return Ok("User added successfully");
                    }
                    else
                    {
                        return BadRequest("Failed to add user");
                    }
                }
            }
        }



        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] UserLogin login)
        {
            string sqlDataSource = _configuration.GetConnectionString("CaseStudyDBCon");
            string query = @"SELECT UserId FROM dbo.Users WHERE Username = @Username AND PasswordHash = @PasswordHash";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@Username", login.username);
                    myCommand.Parameters.AddWithValue("@PasswordHash", login.passwordhash);

                    myCon.Open();
                    SqlDataReader myReader = myCommand.ExecuteReader();

                    if (myReader.HasRows)
                    {
                        // Assuming UserId is an integer, retrieve it from SqlDataReader
                        myReader.Read();
                        int userId = myReader.GetInt32(0);
                        myReader.Close();
                        return Ok(new { userId });
                    }
                    else
                    {
                        return Unauthorized("Invalid username or password");
                    }
                }
            }
        }

        //Transactions Controller Methods

        //ADD
        [HttpPost]
        [Route("AddTransaction")]
        public IActionResult AddTransaction([FromBody] Transaction transaction)
        {
            if (transaction == null)
            {
                return BadRequest("Transaction object is null");
            }

            string sqlDataSource = _configuration.GetConnectionString("CaseStudyDBCon");
            string query = @"INSERT INTO Transactions (UserId, Amount, Description, Category, TransactionDate)
                        VALUES (@UserId, @Amount, @Description, @Category, GETDATE())";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", transaction.UserId);
                    myCommand.Parameters.AddWithValue("@Amount", transaction.Amount);
                    myCommand.Parameters.AddWithValue("@Description", transaction.Description ?? (object)DBNull.Value);
                    myCommand.Parameters.AddWithValue("@Category", transaction.Category ?? (object)DBNull.Value);

                    myCon.Open();
                    int result = myCommand.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return Ok("Transaction added successfully");
                    }
                    else
                    {
                        return BadRequest("Failed to add transaction");
                    }
                }
            }
        }

        //GET
        [HttpGet]
        [Route("GetUserTransactions")]
        public IActionResult GetUserTransactions(int userId)
        {
            string sqlDataSource = _configuration.GetConnectionString("CaseStudyDBCon");
            string query = @"SELECT * FROM Transactions WHERE UserId = @UserId";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", userId);
                    myCon.Open();
                    SqlDataReader myReader = myCommand.ExecuteReader();

                    DataTable transactionsTable = new DataTable();
                    transactionsTable.Load(myReader);
                    myReader.Close();

                    return new JsonResult(transactionsTable);
                }
            }
        }


        //UPDATE
        [HttpPut]
        [Route("UpdateTransaction")]
        public IActionResult UpdateTransaction([FromBody] Transaction transaction)
        {
            if (transaction == null)
            {
                return BadRequest("Transaction object is null");
            }

            string sqlDataSource = _configuration.GetConnectionString("CaseStudyDBCon");
            string query = @"UPDATE Transactions SET Amount = @Amount, Description = @Description, Category = @Category WHERE TransactionId = @TransactionId";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@TransactionId", transaction.TransactionId);
                    myCommand.Parameters.AddWithValue("@Amount", transaction.Amount);
                    myCommand.Parameters.AddWithValue("@Description", transaction.Description ?? (object)DBNull.Value);
                    myCommand.Parameters.AddWithValue("@Category", transaction.Category ?? (object)DBNull.Value);

                    myCon.Open();
                    int result = myCommand.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return Ok("Transaction updated successfully");
                    }
                    else
                    {
                        return BadRequest("Failed to update transaction");
                    }
                }
            }
        }

        //DELETE
        [HttpDelete]
        [Route("DeleteTransaction")]
        public IActionResult DeleteTransaction(int transactionId)
        {
            string sqlDataSource = _configuration.GetConnectionString("CaseStudyDBCon");
            string query = @"DELETE FROM Transactions WHERE TransactionId = @TransactionId";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@TransactionId", transactionId);

                    myCon.Open();
                    int result = myCommand.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return Ok("Transaction deleted successfully");
                    }
                    else
                    {
                        return BadRequest("Failed to delete transaction");
                    }
                }
            }
        }




        //Transfers Controller Methods

        // AddTransfer Endpoint
        [HttpPost]
        [Route("AddTransfer")]
        public IActionResult AddTransfer([FromBody] Transfer transfer)
        {
            if (transfer == null)
            {
                return BadRequest("Transfer object is null");
            }

            string sqlDataSource = _configuration.GetConnectionString("CaseStudyDBCon");
            string query = @"INSERT INTO Transfers (SenderUserId, ReceiverUserId, Amount, Description, TransferDate)
                             VALUES (@SenderUserId, @ReceiverUserId, @Amount, @Description, GETDATE())";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@SenderUserId", transfer.SenderUserId);
                    myCommand.Parameters.AddWithValue("@ReceiverUserId", transfer.ReceiverUserId);
                    myCommand.Parameters.AddWithValue("@Amount", transfer.Amount);
                    myCommand.Parameters.AddWithValue("@Description", transfer.Description ?? (object)DBNull.Value);

                    myCon.Open();
                    int result = myCommand.ExecuteNonQuery();

                    if (result > 0)
                    {
                        return Ok("Transfer added successfully");
                    }
                    else
                    {
                        return BadRequest("Failed to add transfer");
                    }
                }
            }
        }


        //GET
        [HttpGet("GetUserTransfers")]
        public IActionResult GetUserTransfers(int userId)
        {
            string sqlDataSource = _configuration.GetConnectionString("CaseStudyDBCon");
            string query = @"
                            SELECT 
                                t.TransferId, 
                                t.Amount, 
                                t.Description, 
                                t.TransferDate, 
                                t.SenderUserId,
                                t.ReceiverUserId,
                                sender.Username AS SenderUsername, 
                                receiver.Username AS ReceiverUsername
                            FROM Transfers t
                            JOIN Users sender ON t.SenderUserId = sender.UserId
                            JOIN Users receiver ON t.ReceiverUserId = receiver.UserId
                            WHERE t.SenderUserId = @UserId OR t.ReceiverUserId = @UserId";

            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@UserId", userId);
                    myCon.Open();
                    SqlDataReader myReader = myCommand.ExecuteReader();

                    DataTable transfersTable = new DataTable();
                    transfersTable.Load(myReader);
                    myReader.Close();

                    return new JsonResult(transfersTable);
                }
            }
        }




    }
}
