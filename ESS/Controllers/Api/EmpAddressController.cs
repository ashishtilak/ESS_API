using System;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class EmpAddressController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public EmpAddressController()
        {
            _context = new ApplicationDbContext();
        }


        public IHttpActionResult GetEmpAddress(string empUnqId)
        {
            EmpAddressDto empAdd = _context.EmpAddress
                .OrderByDescending(e => e.Counter)
                .Select(Mapper.Map<EmpAddress, EmpAddressDto>)
                .FirstOrDefault(e => e.EmpUnqId == empUnqId);

            if (empAdd == null)
                return BadRequest("Invalid employee.");


            return Ok(empAdd);
        }

        [HttpPost]
        public IHttpActionResult UpdateEmpAddress([FromBody] object requestData)
        {
            EmpAddressDto dto = JsonConvert.DeserializeObject<EmpAddressDto>(requestData.ToString());
            if (!ModelState.IsValid)
                return BadRequest();

            int maxId;
            try
            {
                maxId = _context.EmpAddress.Where(e => e.EmpUnqId == dto.EmpUnqId).Max(e => e.Counter);
            }
            catch
            {
                maxId = 0;
            }

            maxId++;

            EmpAddress empAdd = _context.EmpAddress
                .OrderByDescending(e => e.Counter)
                .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId);


            if (empAdd == null)
            {
                Employees emp = _context.Employees
                    .SingleOrDefault(e => e.EmpUnqId == dto.EmpUnqId);

                if (emp == null)
                    return BadRequest("Invalid employee");
            }
            else
            {
                //Check if data is changed or not
                if (
                    empAdd.HouseNumber == dto.HouseNumber &&
                    empAdd.SocietyName == dto.SocietyName &&
                    empAdd.AreaName == dto.AreaName &&
                    empAdd.LandMark == dto.LandMark &&
                    empAdd.Tehsil == dto.Tehsil &&
                    empAdd.PoliceStation == dto.PoliceStation &&
                    empAdd.PreDistrict == dto.PreDistrict &&
                    empAdd.PreCity == dto.PreCity &&
                    empAdd.PreState == dto.PreState &&
                    empAdd.PrePin == dto.PrePin &&
                    empAdd.PrePhone == dto.PrePhone &&
                    empAdd.PreResPhone == dto.PreResPhone &&
                    empAdd.PreEmail == dto.PreEmail
                )
                {
                    return BadRequest("NO DATA CHANGED.");
                }
            }


            EmpAddress newAdd = new EmpAddress
            {
                EmpUnqId = dto.EmpUnqId,
                Counter = maxId,
                HouseNumber = dto.HouseNumber,
                SocietyName = dto.SocietyName,
                AreaName = dto.AreaName,
                LandMark = dto.LandMark,
                Tehsil = dto.Tehsil,
                PoliceStation = dto.PoliceStation,
                PreDistrict = dto.PreDistrict,
                PreCity = dto.PreCity,
                PreState = dto.PreState,
                PrePin = dto.PrePin,
                PrePhone = dto.PrePhone,
                PreResPhone = dto.PreResPhone,
                PreEmail = dto.PreEmail,
                UpdDt = DateTime.Now,
                HrVerified = false
            };

            _context.EmpAddress.Add(newAdd);

            _context.SaveChanges();

            return Ok();
        }


        [HttpPut]
        public IHttpActionResult UpdateEmpAddressHr([FromBody] object requestData)
        {
            var dto = JsonConvert.DeserializeObject<EmpAddressDto>(requestData.ToString());
            if (!ModelState.IsValid)
                return BadRequest();

            var empAdd = _context.EmpAddress
                .OrderByDescending(e => e.Counter)
                .FirstOrDefault(e => e.EmpUnqId == dto.EmpUnqId && e.Counter == dto.Counter);

            if (empAdd == null)
            {
                return BadRequest("Invalid details");
            }

            empAdd.HouseNumber = dto.HouseNumber;
            empAdd.SocietyName = dto.SocietyName;
            empAdd.AreaName = dto.AreaName;
            empAdd.LandMark = dto.LandMark;
            empAdd.Tehsil = dto.Tehsil;
            empAdd.PoliceStation = dto.PoliceStation;
            empAdd.PreDistrict = dto.PreDistrict;
            empAdd.PreCity = dto.PreCity;
            empAdd.PreState = dto.PreState;
            empAdd.PrePin = dto.PrePin;
            empAdd.PrePhone = dto.PrePhone;
            empAdd.PreResPhone = dto.PreResPhone;
            empAdd.PreEmail = dto.PreEmail;
            empAdd.UpdDt = DateTime.Now;
            empAdd.HrVerified = true;

            empAdd.HrCity = dto.HrCity;
            empAdd.HrSociety = dto.HrSociety;
            empAdd.HrRemarks = dto.HrRemarks;

            _context.SaveChanges();

            return Ok();
        }
    }
}