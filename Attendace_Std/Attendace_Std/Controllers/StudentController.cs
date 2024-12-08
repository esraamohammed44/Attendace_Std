using Attendace_Std.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Attendace_Std.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
       private AppDbContext _repo;
        public StudentController(AppDbContext repoBase)
        {
            _repo = repoBase;
        }
        [HttpPost("Register New Student")]
        public async Task<IActionResult> Register( Student student)
        {
              if (student.Password != student.Repassword)
                {
                    return BadRequest("Passwords Not Repassword .");
                }
            Specialization? res = _repo.specializations.FirstOrDefault(x=>x.Name==student.Specialization_Name);
            Level? l = _repo.levels.FirstOrDefault(x=>x.Name==student.Level_Name);
            
            if (res == null)
            {
                    Specialization s = new Specialization(); 
                    s.Name = student.Specialization_Name;
                   _repo.specializations.Add(s);
                   _repo.SaveChanges();  

            }
            if (l == null)
            {
                Level level = new Level();  
                level.Name = student.Level_Name;
                _repo.levels.Add(level);
                _repo.SaveChanges();

            }
            _repo.Students.Add(student);
              _repo.SaveChanges();  
              return Ok(student);
        }
       
        [HttpGet("Login")]
        public async Task<bool> Login(string Email,string Password)
        {
            Student ? res = _repo.Students.FirstOrDefault(s => s.EmailAddress == Email);
            if (res == null||res.Password != Password )
            {
                return false;
            }
            HttpContext.Session.SetString("UserEmail", res.EmailAddress); 
            HttpContext.Session.SetInt32("UserId", res.Id); 
            return true;
        }
        [HttpGet("Submit Form")]
        public async Task<bool> Attendace(string supervisor_name,string factory_Name,string location )
        {
            Student_attendace student_Attendace = new Student_attendace();  
            var stdId = HttpContext.Session.GetInt32("UserId");
            student_Attendace.Student_Id = (int)stdId;
            Factory ? fac_id=_repo.factories.FirstOrDefault(f => f.Name==factory_Name);
            Supervisor? super_id=_repo.supervisors.FirstOrDefault(f => f.Name==supervisor_name);
            int ch = 0;
            if (fac_id == null)
            {
                ch = 1;
                Factory factory=new Factory();
                factory.Name = factory_Name;
                _repo.factories.Add(factory);
                fac_id = factory;
                _repo.SaveChanges();    
            }
            Std_factory? std_Factory = new Std_factory();
            std_Factory=_repo.std_Factories.FirstOrDefault(x=>x.stdId==(int)stdId&&x.facId==fac_id.Id);
            if(ch==1||std_Factory==null)
            {
                std_Factory= new Std_factory();
                std_Factory.stdId = (int)stdId;
                std_Factory.facId = fac_id.Id;
                _repo.std_Factories.Add(std_Factory);
                ch = 0;
            }
            if (super_id == null)
            {
                ch = 1;
                Supervisor s = new Supervisor();
                s.Name = supervisor_name;
                super_id = s;   
                _repo.supervisors.Add(s);
               
                _repo.SaveChanges();    
            }
            std_supervisor ? std_Supervisor = new std_supervisor();
            std_Supervisor = _repo.std_Supervisors.FirstOrDefault(x=>x.stdId==(int)stdId&&x.suberId==super_id.Id);
            if(ch==1||std_Supervisor==null)
            {
                std_Supervisor=new std_supervisor();
                std_Supervisor.stdId = (int)stdId;
                std_Supervisor.suberId = super_id.Id;
                _repo.std_Supervisors.Add(std_Supervisor);
            }
                student_Attendace.Supervisor_id = super_id.Id;
                student_Attendace.fac_id = fac_id.Id;
                student_Attendace.Location = location;
                
                _repo.student_attendaces.Add(student_Attendace);
                _repo.SaveChanges();    
                return true;    
           
        }

        /* factoryname
  * subervisor
  * location
 * data*/


    }
}
