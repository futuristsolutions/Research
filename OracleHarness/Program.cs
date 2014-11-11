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
        private static readonly string ConnectionString = string.Format(
"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME={1})));User Id={2};Password={3};Pooling=true",
"localhost", "xe", "cs_scheduler", "tesco");
        static void Main(string[] args)
        {
            int algo = 1;
            if (args.Any())
            {
                var parts = args.First().Split(new[] {'='});
                if (parts.First().Contains("dq"))
                {
                    algo = Convert.ToInt32(parts.Last());
                }
                else
                {
                    algo = -1;
                }
            }

            if (algo != -1)
            {
                var action = (algo == 1)
                    ? (Action<int>)(DequeueTest_UsingListen)
                    : DequeueTest_NoListen;
                var tasks = new List<Task>();
                for (int index = 1; index <= 32; index++)
                {
                    int identifier = index;
                    var task = Task.Factory.StartNew(() => action(identifier), TaskCreationOptions.LongRunning);
                    tasks.Add(task);
                }

                Console.WriteLine("{0} - {1}", action.Method.Name, tasks.Count);
                Task.WaitAll(tasks.ToArray());
            }
        }

        private static void DequeueTest_UsingListen(int identifier)
        {
            try
            {
                var aqueueAgent = new [] {new OracleAQAgent(null,"CS_SCHEDULER.SCHEDULE_QUEUE")};
                int index = 0;
                var connection = new OracleConnection(ConnectionString);
                connection.Open();
                while (true)
                {
                    var agent = OracleAQQueue.Listen(connection, aqueueAgent, 1);
                    if (agent != null)
                    {
                        var queue = new OracleAQQueue("SCHEDULE_QUEUE", connection)
                        {
                            MessageType = OracleAQMessageType.Raw,
                            EnqueueOptions =
                            {
                                Visibility = OracleAQVisibilityMode.OnCommit,
                                DeliveryMode = OracleAQMessageDeliveryMode.Persistent
                            }
                        };
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
                        queue.Dispose();
                        transaction.Dispose();
                        queue.Dispose();
                    }
                    ++index;
                    //Console.WriteLine("[{0:D2}] Iteration {1:D4}", identifier, index);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                Console.WriteLine("Identifier : {0}", identifier);
                Environment.Exit(0);
            }
           
        }

        private static void DequeueTest_NoListen(int identifier)
        {
            try
            {
                int index = 0;
                while (true)
                {
                    var connection = new OracleConnection(ConnectionString);
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
                    //Console.WriteLine("[{0:D2}] Iteration {1:D4}", identifier, index);
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
