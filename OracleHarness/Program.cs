using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;

namespace OracleHarness
{
    class Program
    {
        private static string connectionString = string.Format(
"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME={1})));User Id={2};Password={3};Pooling=true;Max Pool Size=250",
"localhost", "xe", "cs_scheduler", "tesco");
        static void Main(string[] args)
        {
            var tasks = new List<Task>();
            for (int i = 0; i < 64; i++)
            {
                int index = i;
                var task = Task.Factory.StartNew(() => OracleQueueTest(index), TaskCreationOptions.LongRunning);
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        private static void OracleQueueTest(int identifier)
        {
            try
            {
                int index = 0;
                while (true)
                {
                    var connection = new OracleConnection(connectionString);
                    var queue = new OracleAQQueue("SCHEDULE_QUEUE", connection)
                    {
                        MessageType = OracleAQMessageType.Raw,
                        EnqueueOptions =
                    {
                        Visibility = OracleAQVisibilityMode.OnCommit,
                        DeliveryMode = OracleAQMessageDeliveryMode.Persistent
                    }
                    };
                    connection.Open();
                    var transaction = connection.BeginTransaction();

                    var dequeueOptions = new OracleAQDequeueOptions
                    {
                        Visibility = OracleAQVisibilityMode.OnCommit,
                        NavigationMode = OracleAQNavigationMode.NextMessage,
                        DequeueMode = OracleAQDequeueMode.Remove,
                        DeliveryMode = OracleAQMessageDeliveryMode.Persistent,
                        Wait = 1
                    };
                    try
                    {
                        queue.Dequeue(dequeueOptions);
                    }
                    catch
                    {
                    }

                    transaction.Commit();
                    transaction.Dispose();
                    queue.Dispose();
                    connection.Dispose();
                    connection.Close();
                    ++index;
                    Console.WriteLine("[{0:D2}] Iteration {1:D4}", identifier, index);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw;
            }
           
        }
    }
}
