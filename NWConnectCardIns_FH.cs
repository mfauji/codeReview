using System;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using System.Data;
using System.Net.Http;
using mimoserviceproject.Models;
using mimoserviceprojectlib;
using mimoserviceprojectobj;
using System.Net;
using System.IO;
using System.Text;
using System.Net.Http.Headers;
using System.Web.Security;
using System.Web.Helpers;

namespace mimoserviceproject.Controllers
{
    /* @Method : NWConnectCardIns_FH()
    * @Param  :{
                "username":"6281288201053",
				"cardNumber":"8000100611355489",
				"udid":"86911260-9050-4E2F-A5C2-F673DAD64E0F",
                "deviceType":""
    			}

    * @Return : {
			"metadata":{
			  "code":"200",
			  "message":"Success !"
			},
			"data":[
				{
					"username":"",
                    "cardNumber":""
				}
			]
		}
    * ----------------------------------------------------
    * Date			    Dev			Remarks
    * ----------------------------------------------------
    * 11 Nov 2022		Herri	    initial
    * ----------------------------------------------------
    * */
    public class NWConnectCardIns_FH : ApiController
    {
        public string Post([NakedBody] string value)
        {
            string strControllerName = this.ControllerContext.RouteData.Values["controller"].ToString();

            #region "Instanceof Write File Log Result"
            WriteFileLogResult writeFileLogResult = new WriteFileLogResult();
            writeFileLogResult.id = Helper.GenerateUniqueID();
            writeFileLogResult.trxDate = Helper.GetDateFormatDate();
            writeFileLogResult.trxTime = Helper.GetDateFormatTime();
            #endregion

            //0. Declaration variable / Instansiasi object
            #region "Declaration Variable"

            String strResult = "";
            JavaScriptSerializer js;
            GeneralLib objGeneralLib = new GeneralLib();
            ResultDecrypt objResultDecrypt = new ResultDecrypt();

            Result objResultDataAccess = new Result();
            ResultNWConnectCardIns_FH obj = new ResultNWConnectCardIns_FH();
            MetaData metaData = new MetaData();
            List<ObjNWConnectCardIns_FH_Res> objDatas = new List<ObjNWConnectCardIns_FH_Res>();
            #endregion

            //1. Decrypt value using private key
            objResultDecrypt = objGeneralLib.DecryptingData(value);
            if (objResultDecrypt.code == Result.NOERROR)
            {
                try
                {
                    //1.1. Instansiasi Object
                    ParamNWConnectCardIns_FH paramValue = new ParamNWConnectCardIns_FH();
                    js = new JavaScriptSerializer();
                    paramValue = js.Deserialize<ParamNWConnectCardIns_FH>(objResultDecrypt.decryptedData);

                    writeFileLogResult.param = paramValue;

                    /* Tulis di sini */
                    #region "Validation"

                    bool isValid = false;

                    if (!string.IsNullOrEmpty(paramValue.username) 
                        && !string.IsNullOrEmpty(paramValue.cardNumber) 
                        && !string.IsNullOrEmpty(paramValue.udid) 
                        && !string.IsNullOrEmpty(paramValue.deviceType)
                        && !string.IsNullOrEmpty(paramValue.birthDate)
                        && !string.IsNullOrEmpty(paramValue.insuranceTp))
                    {
                        isValid = true;
                    }

                    #endregion
                    #region "Flow control"

                    if (isValid)
                    {
                        //ACCESS SP/FN MI

                        objResultDataAccess = new DataAccess().NWConnectCardIns_FH(paramValue);
                        if (objResultDataAccess.ERRORCODE.Equals(Result.NOERROR))
                        {
                            //#. success {code=200}!
                            objDatas = (List<ObjNWConnectCardIns_FH_Res>)objResultDataAccess.TAG;
                            metaData.code = Constan.CONST_RES_CD_SUCCESS;
                            metaData.message = Constan.CONST_RES_MESSAGE_SUCCESS;

                        }
                        else
                        {
                            if (objResultDataAccess.ERRORCODE.Equals(Result.ERROR_EXCEPTION))
                            {
                                //#. error DB {code=500}!
                                metaData.code = Constan.CONST_RES_CD_CATCH;
                                metaData.message = Helper.SetMessageResDB(objResultDataAccess.ERRORCODE, objResultDataAccess.ERRORDESC);
                            }
                            else
                            {
                                //#. error DB {code=600}!
                                metaData.code = objResultDataAccess.ERRORCODE;
                                metaData.message = objResultDataAccess.ERRORDESC;
                            }
                        }

                    }
                    else
                    {
                        //#. error validation {code=400}!
                        metaData.code = Constan.CONST_RES_CD_ERROR;
                        metaData.message = Constan.CONST_RES_MESSAGE_INVALIDINPUT;
                    }

                    #endregion

                }
                catch (Exception ex)
                {
                    //#. error exception {code=500}!
                    metaData.code = Constan.CONST_RES_CD_CATCH;
                    metaData.message = "Catch#1 " + ex.Message;
                }
            }
            else
            {
                //#. error validation {code=400}!
                metaData.code = Constan.CONST_RES_CD_ERROR;
                metaData.message = Constan.CONST_RES_MESSAGE_INVALID_DECRYPTED_DATA;
            }

            //2. return value
            js = new JavaScriptSerializer();

            //2.1. set value to object resultLogin
            obj.metadata = metaData;
            obj.data = objDatas;

            //2.2. serialize object to string json
            strResult = js.Serialize(obj);

            //2.2.1 create log file
            writeFileLogResult.result = obj;
            WriteFileLog.Write(js.Serialize(writeFileLogResult), strControllerName, obj.metadata.code);

            //2.3. encrypted string json using private key
            //strResult = objGeneralLib.EncryptingData(strResult, objResultDecrypt.decryptedKey, objResultDecrypt.decryptedIV);

            //2.4. return value
            return strResult;
        }

    }
}
