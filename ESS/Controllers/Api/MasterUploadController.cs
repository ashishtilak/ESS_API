using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using ESS.Dto;
using ESS.Models;
using Newtonsoft.Json;

namespace ESS.Controllers.Api
{
    public class MasterUploadController : ApiController
    {
        private readonly ApplicationDbContext _context;

        public MasterUploadController()
        {
            _context = new ApplicationDbContext();
        }

        [HttpGet]
        public IHttpActionResult GetObject(string flag, string objectCode)
        {
            switch (flag)
            {
                case "m":
                {
                    var mat = _context.Materials.FirstOrDefault(m => m.MaterialCode == objectCode);
                    if (mat != null)
                        return Ok(Mapper.Map<Materials, MaterialDto>(mat));
                    else
                        return BadRequest("Material not found.");
                }
                case "v":
                {
                    var vend = _context.Vendors.FirstOrDefault(v => v.VendorCode == objectCode);
                    if (vend != null)
                        return Ok(Mapper.Map<Vendors, VendorDto>(vend));
                    else
                        return BadRequest("Vendor not found.");
                }
                default:
                    return BadRequest("Invalid flag.");
            }
        }

        [HttpPost]
        public IHttpActionResult UploadFile([FromBody] object requestData, string empUnqId, string flag)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid model state.");

            try
            {
                switch (flag)
                {
                    case "m":
                        return Ok(UpdateMaterials(requestData, empUnqId));
                    case "v":
                        return Ok(UpdateVendor(requestData, empUnqId));
                    default:
                        return BadRequest("Invalid flag. Use m for material, v for vendor.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest("Error:" + ex);
            }
        }

        private string UpdateMaterials(object requestData, string empUnqId)
        {
            var materials = JsonConvert.DeserializeObject<MaterialDto[]>(requestData.ToString());
            if (materials == null)
                throw new Exception("Invalid format.");

            // Make all validations here:

            // Check for nulls:
            if (materials.Any(m =>
                (m.MaterialCode == null) ||
                (m.MaterialDesc == null) ||
                (m.Uom == null))
            )
                throw new Exception("All data must be filled. No Null allowed.");

            try
            {
                int changed = 0;
                int added = 0;

                foreach (var material in materials)
                {
                    var mat = _context.Materials.FirstOrDefault(m => m.MaterialCode == material.MaterialCode);
                    if (mat == null)
                    {
                        //Length of material code must be 8 characters
                        if (material.MaterialCode.Length != 8)
                            throw new Exception("Material code length must be 8 characters. Check: " +
                                                material.MaterialCode);
                        material.UpdDt = DateTime.Now;
                        material.UpdUser = empUnqId;
                        _context.Materials.Add(Mapper.Map<MaterialDto, Materials>(material));
                        added++;
                    }
                    else
                    {
                        mat.MaterialDesc = material.MaterialDesc;
                        mat.Uom = material.Uom;
                        mat.HsnCode = material.HsnCode;
                        mat.UpdDt = DateTime.Now;
                        mat.UpdUser = empUnqId;
                        changed++;
                    }
                }

                _context.SaveChanges();
                return ("Total Added: " + added + ", Changed: " + changed);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex);
            }
        }

        private string UpdateVendor(object requestData, string empUnqId)
        {
            var vendors = JsonConvert.DeserializeObject<VendorDto[]>(requestData.ToString());
            if (vendors == null)
                throw new Exception("Invalid format.");

            // Make all validations here:

            // Check for nulls:
            if (vendors.Any(v =>
                    (v.VendorCode == null) ||
                    (v.VendorAddress1 == null)
                )
            )
                throw new Exception("All data must be filled. No Null allowed.");

            try
            {
                int changed = 0;
                int added = 0;

                foreach (var vendor in vendors)
                {
                    var ven = _context.Vendors.FirstOrDefault(v => v.VendorCode == vendor.VendorCode);
                    if (ven == null)
                    {
                        //Length of material code must be 8 characters
                        if (vendor.VendorCode.Length != 7)
                            throw new Exception("Vendor code length must be 7 characters. Check: " +
                                                vendor.VendorCode);
                        vendor.UpdDt = DateTime.Now;
                        vendor.UpdUser = empUnqId;
                        _context.Vendors.Add(Mapper.Map<VendorDto, Vendors>(vendor));
                        added++;
                    }
                    else
                    {
                        ven.VendorName = vendor.VendorName;
                        ven.VendorAddress1 = vendor.VendorAddress1;
                        ven.VendorAddress2 = vendor.VendorAddress2;
                        ven.VendorAddress3 = vendor.VendorAddress3;
                        ven.UpdDt = DateTime.Now;
                        ven.UpdUser = empUnqId;
                        changed++;
                    }
                }

                _context.SaveChanges();
                return ("Total Added: " + added + ", Changed: " + changed);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex);
            }
        }
    }
}