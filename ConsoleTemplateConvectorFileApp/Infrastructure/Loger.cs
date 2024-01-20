using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateWorkApp
{
    internal class Loger
    {
        private string _operationName;
        private DateTime _startOfOperation;
        private DateTime _endOfOperation;
        private TimeSpan _operationExecutionTime;
        public string OperationName
        {
            get { return _operationName; }
        }
        public DateTime StartOfOperation
        {
            get { return _startOfOperation; }
        }
        public DateTime EndOfOperation
        {
            get { return _endOfOperation; }
            set { _endOfOperation = value; }
        }
        public TimeSpan OperationExecutionTime
        {
            get { return _operationExecutionTime; }
        }
        public void StartLog(DateTime startOfOperation, string operationName)
        {
            _startOfOperation = startOfOperation;
            _operationName = operationName;
        }
        public void FinishLog(DateTime endOfOperation)
        {
            EndOfOperation = endOfOperation;
            _operationExecutionTime = _endOfOperation - _startOfOperation;
            SqlLiteClient.SaveDataAboutTheOperationВeingPerformed(_operationName, _startOfOperation, _endOfOperation, _operationExecutionTime);
        }
        public Loger(DateTime startOfOperation, string operationName)
        {
            StartLog(startOfOperation, operationName);
        }
    }
}
