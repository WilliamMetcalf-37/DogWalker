using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using DogWalker.Models;

namespace DogWalker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalkerController : ControllerBase
    {
        private readonly IConfiguration _config;

        public WalkerController(IConfiguration config)
        {
            _config = config;
        }

        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? NeighborhoodId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT w.Id as WalkerId, w.Name, w.NeighborhoodId, ws.id as WalkId, ws.WalkerId as WalkWalkerId, ws.Date, ws.Duration, ws.DogId, d.Id as DID, d.Name as DogName, d.Breed
                       FROM Walker w 
                        left join Walks ws 
                        on w.Id = ws.WalkerId 
                        left join dog d on d.Id = ws.DogId
                        Where 1=1";

                    if (NeighborhoodId != null)
                    {
                        cmd.CommandText += " AND NeighborhoodId = @neighborhoodId";
                        cmd.Parameters.Add(new SqlParameter("@neighborhoodId", NeighborhoodId));
                    }

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Walker> walkers = new List<Walker>();
                    while (reader.Read())
                    {



                        var existingWalker = walkers.FirstOrDefault(walker => walker.Id == reader.GetInt32(reader.GetOrdinal("WalkerId")));

                        if (existingWalker == null)
                        {
                               var walker = new Walker
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("WalkerId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                                Walks = new List<Walk>()

                            };

                                    walker.Walks.Add(new Walk()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("WalkId")),
                              WalkerId = reader.GetInt32(reader.GetOrdinal("WalkWalkerId")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                                Dog = new Dog()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("DID")),
                                    Name = reader.GetString(reader.GetOrdinal("DogName")),
                                   Breed = reader.GetString(reader.GetOrdinal("Breed"))

                                }

                            });
                            walkers.Add(walker);
                        }
                        else
                        {
                            existingWalker.Walks.Add(new Walk()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("WalkId")),
                                WalkerId = reader.GetInt32(reader.GetOrdinal("WalkWalkerId")),
                                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                                Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                                DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                                Dog = new Dog()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("DID")),
                                    Name = reader.GetString(reader.GetOrdinal("DogName")),
                                    Breed = reader.GetString(reader.GetOrdinal("Breed"))

                                }

                            });
                        }


                        
                        
                    }
                    
                    reader.Close();

                    return Ok(walkers);
                }
            }
        }

        [HttpGet("{id}", Name = "GetWalker")]
        public async Task<IActionResult> Get([FromRoute] int id,
            [FromQuery] string include)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name, NeighborhoodId FROM Walker
                        WHERE Id = @id";

                    //if (include == "walks")
                    //{
                    //    cmd.CommandText = @"SELECT w.Id as WalkerId, w.Name, w.NeighborhoodId, ws.id as WalkId, ws.WalkerId as WalkWalkerId, ws.Date, ws.Duration, ws.DogId, d.Id as DID, d.Name as DogName, d.Breed
                    //   FROM Walker w 
                    //    inner join Walks ws 
                    //    on w.Id = ws.WalkerId left join dog d on d.Id = ws.DogId
                    //    WHERE w.Id = @id";
 

                    //}
                    SqlDataReader reader = cmd.ExecuteReader();
                    cmd.Parameters.Add(new SqlParameter("@id", id));


                   


                    

                    Walker walker = null;
                    //if (reader.Read() && include == "walks")
                    //{
                    //    walker = new Walker
                    //    {
                    //        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    //        Name = reader.GetString(reader.GetOrdinal("Name")),
                    //        NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),
                    //        Walks = new List<Walk>()

                    //    };
                    //    walker.Walks.Add(new Walk()
                    //    {
                    //        Id = reader.GetInt32(reader.GetOrdinal("WalkId")),
                    //        WalkerId = reader.GetInt32(reader.GetOrdinal("WalkWalkerId")),
                    //        Date = reader.GetDateTime(reader.GetOrdinal("Date")),
                    //        Duration = reader.GetInt32(reader.GetOrdinal("Duration")),
                    //        DogId = reader.GetInt32(reader.GetOrdinal("DogId")),
                    //        Dog = new Dog()
                    //        {
                    //            Id = reader.GetInt32(reader.GetOrdinal("DID")),
                    //            Name = reader.GetString(reader.GetOrdinal("DogName")),
                    //            Breed = reader.GetString(reader.GetOrdinal("Breed"))

                    //        }

                    //    });
                    //}else
                    //{
                    if (reader.Read())
                    {
                            walker = new Walker
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            NeighborhoodId = reader.GetInt32(reader.GetOrdinal("NeighborhoodId")),

                        };
                    }
                        

                    //}
                    
                    reader.Close();

                    return Ok(walker);
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Walker walker)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Walker (Name, NeighborhoodId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@Name, @NeighborhoodId)";
                    cmd.Parameters.Add(new SqlParameter("@Name", walker.Name));
                    cmd.Parameters.Add(new SqlParameter("@NeighborhoodId", walker.NeighborhoodId));
                   
                    int newId = (int)cmd.ExecuteScalar();
                    walker.Id = newId;
                    return CreatedAtRoute("GetWalker", new { id = newId }, walker);
                }
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put([FromRoute] int id, [FromBody] Walker walker)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Walker
                                            SET Name = @Name,
                                                NeighborhoodId = @NeighborhoodId

                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@Name", walker.Name));
                        cmd.Parameters.Add(new SqlParameter("@NeighborhoodId", walker.NeighborhoodId));
                        cmd.Parameters.Add(new SqlParameter("@id", walker.Id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!WalkerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Walker WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!WalkerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool WalkerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name, NeighborhoodId
                        FROM Walker
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();
                    return reader.Read();
                }
            }
        }
    }
}