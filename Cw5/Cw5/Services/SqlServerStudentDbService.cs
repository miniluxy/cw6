using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Cw5.DTOs.Requests;
using Cw5.DTOs.Responses;
using Cw5.Models;

namespace Cw5.Services
{
    public class SqlServerStudentDbService : IStudentDbService      //nie wiem o co chodzi przecież jest...
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s19263;Integrated Security=True";

        public SqlServerStudentDbService(/*.. */ )
        {

        }

        public EnrollStudentResponse EnrollStudent(EnrollStudentRequest request)
        {
            //DTOs - Data Transfer Objects
            //Request models
            //==mapowanie==
            //Modele biznesowe/encje (entity)
            //==mapowanie==
            //Response models

            //var st = new Student();
            //st.FirstName = request.FirstName;
            //...
            //...
            //Micro ORM object-relational mapping
            //problemami - impedance mismatch

            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                var tran = con.BeginTransaction();
                try
                {
                    com.CommandText = "Select * from Student where IndexNumber=@index";
                    com.Parameters.AddWithValue("@index", request.IndexNumber);
                    var dr = com.ExecuteReader();

                    if (dr.Read())
                        return null;

                    dr.Close();
                    //1. Czy studia istnieja?
                    com.CommandText = "select IdStudy from Studies where name=@name";
                    com.Parameters.AddWithValue("name", request.Studies);

                    dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        tran.Rollback();
                        return BadRequest("Studia nie istnieja");
                        //...
                    }
                    int idstudies = int.Parse(dr["IdStudies"].ToString());
                    dr.Close();

                    com.CommandText = "Select IdEnrollment from Enrollment where StartDate=(select Max(StartDate) from Enrollment where IdStudy=@id and Semester=1) and IdStudy=@id and Semester=1";
                    com.Parameters.AddWithValue("@id", idstudies);
                    bool dataPresent = false;
                    int IdEnrollment = 0;
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        dataPresent = true;
                        IdEnrollment = int.Parse(dr["IdEnrollment"].ToString());
                    }
                    dr.Close();
                    SqlTransaction transaction = con.BeginTransaction();
                    try
                    {

                        com.Transaction = transaction;

                        EnrollStudentResponse response = new EnrollStudentResponse();
                        if (dataPresent)
                        {
                            com.CommandText = "Insert into Student Values(@IndexNumber,@FirstName,@LastName,@BirthDate,@IdEnrollment)";
                            com.Parameters.AddWithValue("@IndexNumber", request.IndexNumber);
                            com.Parameters.AddWithValue("@FirstName", request.FirstName);
                            com.Parameters.AddWithValue("@LastName", request.LastName);
                            com.Parameters.AddWithValue("@BirthDate", request.Birthdate);
                            com.Parameters.AddWithValue("@IdEnrollment", IdEnrollment);
                            com.ExecuteNonQuery();

                            com.Parameters.Clear();
                            com.CommandText = "select IdEnrollment,Semester,StartDate,Name from Enrollment inner join Studies on Enrollment.IdStudy=Studies.IdStudy where IdEnrollment=@IdEnrollment";
                            com.Parameters.AddWithValue("@IdEnrollment", IdEnrollment);
                            dr = com.ExecuteReader();
                            if (dr.Read())
                            {
                                response.IdEnrollment = int.Parse(dr["IdEnrollment"].ToString());
                                response.Semester = int.Parse(dr["Semester"].ToString());
                                response.Studies = dr["Name"].ToString();
                                response.StartDate = DateTime.Parse(dr["StartDate"].ToString());
                                dr.Close();
                                transaction.Commit();
                                transaction.Dispose();
                                return response;
                            }
                            return null;
                        }
                        else
                        {
                            dr.Close();
                            com.CommandText = "Insert into Enrollment Values((Select ISNULL(Max(IdEnrollment),0)+1 from Enrollment),1,@IdStudy,(SELECT CONVERT(date, getdate())))";
                            com.Parameters.AddWithValue("@IdStudy", idstudies);
                            if (com.ExecuteNonQuery() == 1)
                            {
                                com.CommandText = "Insert into Student Values(@IndexNumber,@FirstName,@LastName,@BirthDate,(Select Max(IdEnrollment) from Enrollment))";
                                com.Parameters.AddWithValue("@IndexNumber", request.IndexNumber);
                                com.Parameters.AddWithValue("@FirstName", request.FirstName);
                                com.Parameters.AddWithValue("@LastName", request.LastName);
                                com.Parameters.AddWithValue("@BirthDate", request.Birthdate);
                                com.ExecuteNonQuery();

                                com.CommandText = "select IdEnrollment,Semester,StartDate,Name from Enrollment inner join Studies on Enrollment.IdStudy=Studies.IdStudy where IdEnrollment=(Select MAX(IdEnrollment) from Enrollment)";
                                dr = com.ExecuteReader();
                                if (dr.Read())
                                {
                                    response.IdEnrollment = int.Parse(dr["IdEnrollment"].ToString());
                                    response.Semester = int.Parse(dr["Semester"].ToString());
                                    response.Studies = dr["Name"].ToString();
                                    response.StartDate = DateTime.Parse(dr["StartDate"].ToString());
                                    dr.Close();
                                    transaction.Commit();
                                    transaction.Dispose();
                                    return response;
                                }
                                return null;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Dispose();
                        return null;
                    }
                }
                catch (SqlException exc)
                {
                    return null;
                }
            }
        }
        public PromoteStudentResponse PromoteStudent(PromoteStudentRequest request)
        {
            using (SqlConnection con = new SqlConnection(ConString))
            using (SqlCommand com = new SqlCommand())
            {
                try
                {
                    com.Connection = con;
                    com.CommandText = "select * from Enrollment inner join Studies on Enrollment.IdStudy=Study.IdStudy where Name=@StudyName and Semester=@Semester";
                    com.Parameters.AddWithValue("@StudiesName", request.Studies);
                    com.Parameters.AddWithValue("@Semester", request.Semester);
                    con.Open();
                    var dr = com.ExecuteReader();
                    if (!dr.Read())
                    {
                        return null;
                    }
                    dr.Close();
                    com.Parameters.Clear();
                    com.CommandText = "PromoteStudents";
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@Studies", request.Studies);
                    com.Parameters.AddWithValue("@Semester", request.Semester);
                    dr = com.ExecuteReader();
                    if (dr.Read())
                    {
                        PromoteStudentResponse response = new PromoteStudentResponse();
                        response.IdEnrollment = int.Parse(dr["IdEnrollment"].ToString());
                        response.Semester = int.Parse(dr["Semester"].ToString());
                        response.Studies = dr["Name"].ToString();
                        response.StartDate = DateTime.Parse(dr["StartDate"].ToString());
                        dr.Close();
                        return response;
                    }
                    else
                    {
                        return null;
                    }
                }
                catch (SqlException ex)
                {
                    return null;
                }
            }
        }



        private EnrollStudentResponse BadRequest(string v)
        {
            throw new NotImplementedException();
        }
    }
}