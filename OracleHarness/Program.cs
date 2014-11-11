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
        private const int NoMessagesErrorCode = 25228;
        static void Main(string[] args)
        {
            int algo = 1;
            int delay = 0;
            if (args.Any())
            {
                var parts = args.First().Split(new[] {'='});
                if (parts.First().Contains("dq"))
                {
                    algo = Convert.ToInt32(parts.Last());
                }
                else if (parts.First().Contains("eq"))
                {
                    algo = -1;
                    if (parts.Count() == 2)
                    {
                        delay = Convert.ToInt32(parts.Last());
                    }
                }
                else
                {
                    algo = -1;
                }
            }

            if (algo != -1)
            {
                DequeueTest(algo);
            }
            else
            {
                EnqueueData(delay);
            }
        }
        
        private static void DequeueTest(int algo)
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

        private static void EnqueueData(int delay)
        {
            Console.WriteLine("Message delay seconds {0}",delay);
            var connection = new OracleConnection(ConnectionString);
            connection.Open();
            for (int index = 1; index <= 5; index++)
            {
                var data = string.Format("Test message : {0} - {1}", Guid.NewGuid().ToString(), index);
                var message = new OracleAQMessage
                {

                    Payload = Encoding.UTF8.GetBytes(data),
                    Correlation = index.ToString(),
                    Delay = 0
                };

                var queue = new OracleAQQueue("SCHEDULE_QUEUE", connection)
                {
                    MessageType = OracleAQMessageType.Raw,
                    EnqueueOptions =
                    {
                        Visibility = OracleAQVisibilityMode.OnCommit,
                        DeliveryMode = OracleAQMessageDeliveryMode.Persistent
                    }
                };
                Console.WriteLine("Enqueueing {0}",data);
                var transaction = connection.BeginTransaction();
                queue.Enqueue(message);
                transaction.Commit();
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
                        Dequeue(connection);
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
                var connection = new OracleConnection(ConnectionString);
                connection.Open();
                while (true)
                {
                    Dequeue(connection);
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


        private static void Dequeue(OracleConnection connection)
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
                var message = queue.Dequeue(dequeueOptions);
                if (message != null)
                {
                    var data = Encoding.UTF8.GetString((byte[])message.Payload);
                    Console.WriteLine(data);
                }
            }
            catch (OracleException ex)
            {
                if (ex.Number != NoMessagesErrorCode)
                    throw;
            }
            transaction.Commit();
            transaction.Dispose();
            queue.Dispose();
        }
    }
}
