namespace Human.Chrs.Domain.CommonModels
{
    public class CommonResult
    {
        public bool Success => errors.Count == 0;

        public string Errors => string.Join(",", errors);

        private List<string> errors = new List<string>();

        public void AddError(string errorMessage)
        {
            errors.Add(errorMessage);
        }

        public void AddErrors(IEnumerable<string> errorMessages)
        {
            errors.AddRange(errorMessages);
        }

        public string ErrorMessage(string separator)
        {
            return string.Join(separator, errors.Select(error => error));
        }
    }

    public class CommonResult<T> : CommonResult
    {
        public T Data { get; set; }
    }
}