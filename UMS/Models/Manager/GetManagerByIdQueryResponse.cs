﻿namespace UMS.Models.Manager;

public class GetManagerByIdQueryResponse
{
    public int Id { get; set; }
    public string FullName { get; set; } =  string.Empty;
    public string Email { get; set; } =  string.Empty;
    public string Username { get; set; } =  string.Empty;
    public string Password { get; set; } =  string.Empty;
    public int DesignationId { get; set; } 
    public string Designation { get; set; } =  string.Empty;
}