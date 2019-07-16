
using Share;
using Share.Logs;

namespace AccountServer.AccountServer.ID
{
    public class IDGenerator : Singleton<IDGenerator>
    {
        private const long INITIAL_TIME_STAMP = 1483200000000L;
        private const long WORKER_ID_BITS = 5L;
        private const long DATACENTER_ID_BITS = 5L;
        private const long MAX_WORKER_ID = ~(-1L << (int)WORKER_ID_BITS);
        private const long MAX_DATACENTER_ID = ~(-1L << (int)DATACENTER_ID_BITS);
        private const long SEQUENCE_BITS = 12L;
        private const long WORKER_ID_OFFSET = SEQUENCE_BITS;
        private const long DATACENTER_ID_OFFSET = SEQUENCE_BITS + WORKER_ID_BITS;
        private const long TIMESTAMP_OFFSET = SEQUENCE_BITS + WORKER_ID_BITS + DATACENTER_ID_BITS;
        private const long SEQUENCE_MASK = ~(-1L << (int)SEQUENCE_BITS);


        private long m_WorkerID;
        private long m_DataCenterID;
        private long m_Sequence;
        private long m_LastTimestamp;
        private object m_Lock;



        public IDGenerator()
        {
            m_WorkerID = 0;
            m_DataCenterID = 0;
            m_Sequence = 0;
            m_LastTimestamp = -1;

            m_Lock = new object();
        }

        public void Init(long worker_id, long datacenter_id)
        {
            if (worker_id > MAX_WORKER_ID || worker_id < 0)
            {
                LogManager.Error("Worker ID can not greater than " + MAX_WORKER_ID.ToString() +
                                 " or less than 0.");
            }

            if (datacenter_id > MAX_DATACENTER_ID || datacenter_id < 0)
            {
                LogManager.Error("DataCenter ID can not greater than " + MAX_DATACENTER_ID +
                                 " or less than 0.");
            }

            m_WorkerID = worker_id;
            m_DataCenterID = datacenter_id;
        }


        public long NextID()
        {
            long cur_timestamp = Time.GetCurMilliseconds();

            lock (m_Lock)
            {
                if (cur_timestamp < m_LastTimestamp)
                {
                    LogManager.Error("Clock moved backwards. Refusing to generate id for " +
                                     (m_LastTimestamp - cur_timestamp).ToString() + " milliseconds.");
                }

                if (cur_timestamp == m_LastTimestamp)
                {
                    m_Sequence = (m_Sequence + 1) & SEQUENCE_MASK;

                    if (0 == m_Sequence)
                    {
                        cur_timestamp = TillNextMillisecond(m_LastTimestamp);
                    }
                }
                else
                {
                    m_Sequence = 0L;
                }

                m_LastTimestamp = cur_timestamp;
            }

            return ((cur_timestamp - INITIAL_TIME_STAMP) << (int)TIMESTAMP_OFFSET) |
                    (m_DataCenterID << (int)DATACENTER_ID_OFFSET) |
                    (m_WorkerID << (int)WORKER_ID_OFFSET) |
                    m_Sequence;
        }


        private long TillNextMillisecond(long last_timestamp)
        {
            long cur_timestamp = Time.GetCurMilliseconds();

            while (cur_timestamp <= last_timestamp)
            {
                cur_timestamp = Time.GetCurMilliseconds();
            }

            return cur_timestamp;
        }
    }
}
