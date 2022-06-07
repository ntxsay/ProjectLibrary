using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppHelpers
{
    public class OperationState
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }

    public class IdOperationState : OperationState
    {
        public long Id { get; set; }
        public Guid Guid { get; set; }
    }

    //public class OperationState : OperationState
    //{
    //    public long Id { get; set; }
    //    public Guid Guid { get; set; }
    //    public string Title { get; set; }
    //    public string Glyph { get; set; } = "\uE9CE"; //UnKnow
    //    public object Result { get; set; }
    //}

    public class OperationState<T> where T : class
    {
        public OperationState()
        {
        }

        public OperationState(T parameter)
        {
            Result = parameter;
        }

        public OperationState(IEnumerable<T> parameters)
        {
            ResultList = parameters?.ToList();
        }

        public long Id { get; set; }
        public Guid Guid { get; set; }
        public bool IsSuccess { get; set; }
        public string Title { get; set; }
        public string Glyph { get; set; } = "\uE9CE"; //UnKnow
        public string Message { get; set; }
        public T Result { get; set; }
        public List<T> ResultList { get; set; }
    }
}
