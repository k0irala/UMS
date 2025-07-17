using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace UMS;


public class ApiResponse<T>
{
        private List<Errors> _fieldErrors;
        public int code { get; set; } // e.g., 200, 404
        public string message { get; set; } = string.Empty;   // "Users fetched successfully", etc.
        public T data { get; set; }   
        public List<Errors>? errors
        {
                get => _fieldErrors ?? new();
                set => _fieldErrors = value;
        }// Your actual data (List<UserDto>, etc.)
        public MetaData meta { get; set; }            // Additional metadata
}
public class Errors
{
        [JsonProperty(Order = 0)]
        [JsonPropertyOrder(0)]
        public string? errorCode { get; set; }
        [JsonProperty(Order = 1)]
        [JsonPropertyOrder(1)]
        public string? errorMessage { get; set; }
}

public class MetaData
{
        public Pagination pagination { get; set; }
        public Sort sort  { get; set; }
}

public class Pagination
{
        public int totalItems { get; set; }
        public int currentPage { get; set; }
        public int pageSize { get; set; }
        public int totalPages { get; set; }
        public bool hasNextPage { get; set; }
}

public class Sort
{
        public string field  { get; set; }
        public string order  { get; set; }
}