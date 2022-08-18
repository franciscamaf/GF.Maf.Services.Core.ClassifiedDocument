using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Maf.Services.Core.ClassifiedDocument.Model
{
    public class FieldValueList
    {
        #region PAGARE
        public string PAGA_NRO_VALUE { get; set; }
        public string PAGA_NRO_VALUE_
        {
            get
            {
                if (this.PAGA_NRO_VALUE == "" || this.PAGA_NRO_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.PAGA_NRO_VALUE;
            }
        }
        public string PAGA_NRO_VALID { get; set; }


        public string PAGA_MONTO_LETRAS_VALUE { get; set; }
        public string PAGA_MONTO_LETRAS_VALUE_
        {
            get
            {
                if (this.PAGA_MONTO_LETRAS_VALUE == "" || this.PAGA_MONTO_LETRAS_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.PAGA_MONTO_LETRAS_VALUE;
            }
        }
        public string PAGA_MONTO_LETRAS_VALID { get; set; }


        public string PAGA_NUM_CUOTAS_VALUE { get; set; }
        public string PAGA_NUM_CUOTAS_VALUE_
        {
            get
            {
                if (this.PAGA_NUM_CUOTAS_VALUE == "" || this.PAGA_NUM_CUOTAS_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.PAGA_NUM_CUOTAS_VALUE;
            }
        }
        public string PAGA_NUM_CUOTAS_VALID { get; set; }


        public string PAGA_VALOR_CUOTAS_VALUE { get; set; }
        public string PAGA_VALOR_CUOTAS_VALUE_
        {
            get
            {
                if (this.PAGA_VALOR_CUOTAS_VALUE == "" || this.PAGA_VALOR_CUOTAS_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.PAGA_VALOR_CUOTAS_VALUE;
            }
        }
        public string PAGA_VALOR_CUOTAS_VALID { get; set; }


        public string PAGA_TASA_INTERES_VALUE { get; set; }
        public string PAGA_TASA_INTERES_VALUE_
        {
            get
            {
                if (this.PAGA_TASA_INTERES_VALUE == "" || this.PAGA_TASA_INTERES_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.PAGA_TASA_INTERES_VALUE;
            }
        }
        public string PAGA_TASA_INTERES_VALID { get; set; }


        public string PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALUE { get; set; }
        public string PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALUE_
        {
            get
            {
                if (this.PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALUE == "" || this.PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALUE;
            }
        }
        public string PAGA_FECHA_VENCIMIENTO_CUOTA_1_VALID { get; set; }


        public string PAGA_CUOTA_DORSO_VALUE { get; set; }
        public string PAGA_CUOTA_DORSO_VALUE_
        {
            get
            {
                if (this.PAGA_CUOTA_DORSO_VALUE == "" || this.PAGA_CUOTA_DORSO_VALUE is null)
                {
                    return Constant.stringZero;

                }
                else return this.PAGA_CUOTA_DORSO_VALUE;
            }
        }
        public string PAGA_CUOTA_DORSO_VALID { get; set; }


        public string PAGA_DEUDOR_NOMBRE_VALUE { get; set; }
        public string PAGA_DEUDOR_NOMBRE_VALUE_
        {
            get
            {
                if (this.PAGA_DEUDOR_NOMBRE_VALUE == "" || this.PAGA_DEUDOR_NOMBRE_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.PAGA_DEUDOR_NOMBRE_VALUE;
            }
        }
        public string PAGA_DEUDOR_NOMBRE_VALID { get; set; }


        public string PAGA_DEUDOR_NACION_VALUE { get; set; }
        public string PAGA_DEUDOR_NACION_VALUE_
        {
            get
            {
                if (this.PAGA_DEUDOR_NACION_VALUE == "" || this.PAGA_DEUDOR_NACION_VALUE is null)
                {
                    return Constant.stringNinetyNine;

                }
                else if (this.PAGA_DEUDOR_NACION_VALUE == Constant.nationalityChilean)
                {
                    return Constant.stringOne;
                }
                else
                {
                    return Constant.stringTwo;
                }
            }
        }
        public string PAGA_DEUDOR_NACION_VALID { get; set; }


        public string PAGA_DEUDOR_RUT_VALUE { get; set; }
        public string PAGA_DEUDOR_RUT_VALUE_
        {
            get
            {
                if (this.PAGA_DEUDOR_RUT_VALUE == "" || this.PAGA_DEUDOR_RUT_VALUE is null)
                {
                    return Constant.stringZero;

                }
                else return this.PAGA_DEUDOR_RUT_VALUE.Replace("-", "");
            }
        }
        public string PAGA_DEUDOR_RUT_VALID { get; set; }


        public string PAGA_FIRMA_DEUDOR_VALUE { get; set; }
        public string PAGA_FIRMA_DEUDOR_VALUE_
        {
            get
            {
                if (this.PAGA_FIRMA_DEUDOR_VALUE == "" || this.PAGA_FIRMA_DEUDOR_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.PAGA_FIRMA_DEUDOR_VALUE;
            }
        }
        public string PAGA_FIRMA_DEUDOR_VALID { get; set; }


        public string PAGA_DEUDOR_LUGAR_FIRMA_VALUE { get; set; }
        public string PAGA_DEUDOR_LUGAR_FIRMA_VALUE_
        {
            get
            {
                if (this.PAGA_DEUDOR_LUGAR_FIRMA_VALUE == "" || this.PAGA_DEUDOR_LUGAR_FIRMA_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.PAGA_DEUDOR_LUGAR_FIRMA_VALUE;
            }
        }
        public string PAGA_DEUDOR_LUGAR_FIRMA_VALID { get; set; }


        public string PAGA_FECHA_FIRMA_VALUE { get; set; }
        public string PAGA_FECHA_FIRMA_VALUE_
        {
            get
            {
                if (this.PAGA_FECHA_FIRMA_VALUE == "" || this.PAGA_FECHA_FIRMA_VALUE is null)
                {
                    return Constant.TypeDate;
                }
                else return Convert.ToDateTime(this.PAGA_FECHA_FIRMA_VALUE).ToString("yyyy-MM-dd");
            }
        }
        public string PAGA_FECHA_FIRMA_VALID { get; set; }


        public string PAGA_CODEU_NOMBRE_VALUE { get; set; }
        public string PAGA_CODEU_NOMBRE_VALUE_
        {
            get
            {
                if (this.PAGA_CODEU_NOMBRE_VALUE == "" || this.PAGA_CODEU_NOMBRE_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.PAGA_CODEU_NOMBRE_VALUE;
            }
        }
        public string PAGA_CODEU_NOMBRE_VALID { get; set; }


        public string PAGA_CODEU_NACION_VALUE { get; set; }
        public string PAGA_CODEU_NACION_VALUE_
        {
            get
            {
                if (this.PAGA_CODEU_NACION_VALUE == "" || this.PAGA_CODEU_NACION_VALUE is null)
                {
                    return Constant.stringNinetyNine;

                }
                else if (this.PAGA_CODEU_NACION_VALUE == Constant.nationalityChilean)
                {
                    return Constant.stringOne;
                }
                else
                {
                    return Constant.stringTwo;
                }

            }
        }
        public string PAGA_CODEU_NACION_VALID { get; set; }


        public string PAGA_CODEU_RUT_VALUE { get; set; }
        public string PAGA_CODEU_RUT_VALUE_
        {
            get
            {
                if (this.PAGA_CODEU_RUT_VALUE == "" || this.PAGA_CODEU_RUT_VALUE is null)
                {
                    return Constant.stringZero;

                }
                else return this.PAGA_CODEU_RUT_VALUE.Replace("-", "");
            }
        }
        public string PAGA_CODEU_RUT_VALID { get; set; }


        public string PAGA_CODEU2_NOMBRE_VALUE { get; set; }
        public string PAGA_CODEU2_NOMBRE_VALUE_
        {
            get
            {
                if (this.PAGA_CODEU2_NOMBRE_VALUE == "" || this.PAGA_CODEU2_NOMBRE_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.PAGA_CODEU2_NOMBRE_VALUE;
            }
        }
        public string PAGA_CODEU2_NOMBRE_VALID { get; set; }


        public string PAGA_CODEU2_NACION_VALUE { get; set; }
        public string PAGA_CODEU2_NACION_VALUE_
        {
            get
            {
                if (this.PAGA_CODEU2_NACION_VALUE == "" || this.PAGA_CODEU2_NACION_VALUE is null)
                {
                    return Constant.stringNinetyNine;

                }
                else if (this.PAGA_CODEU2_NACION_VALUE == Constant.nationalityChilean)
                {
                    return Constant.stringOne;
                }
                else
                {
                    return Constant.stringTwo;
                }
            }
        }
        public string PAGA_CODEU2_NACION_VALID { get; set; }


        public string PAGA_CODEU2_RUT_VALUE { get; set; }
        public string PAGA_CODEU2_RUT_VALUE_
        {
            get
            {
                if (this.PAGA_CODEU2_RUT_VALUE == "" || this.PAGA_CODEU2_RUT_VALUE is null)
                {
                    return Constant.stringZero;

                }
                else return this.PAGA_CODEU2_RUT_VALUE.Replace("-", "");
            }
        }
        public string PAGA_CODEU2_RUT_VALID { get; set; }


        public string PAGA_MONTO_NUM_VALUE { get; set; }
        public string PAGA_MONTO_NUM_VALUE_
        {
            get
            {
                if (this.PAGA_MONTO_NUM_VALUE == "" || this.PAGA_MONTO_NUM_VALUE is null)
                {
                    return Constant.stringZero;

                }
                else return this.PAGA_MONTO_NUM_VALUE;
            }
        }
        public string PAGA_MONTO_NUM_VALID { get; set; }
        #endregion


        #region MANDATO

        public string MANDATO_PREND_ESP_MAN_RUT_VALUE { get; set; }
        public string MANDATO_PREND_ESP_MAN_RUT_VALUE_
        {
            get
            {
                if (this.MANDATO_PREND_ESP_MAN_RUT_VALUE == "" || this.MANDATO_PREND_ESP_MAN_RUT_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.MANDATO_PREND_ESP_MAN_RUT_VALUE.Replace("-", ""); ;
            }
        }
        public string MANDATO_PREND_ESP_MAN_RUT_VALID { get; set; }


        public string MANDATO_ESP_MAN_NOM_VALUE { get; set; }
        public string MANDATO_ESP_MAN_NOM_VALUE_
        {
            get
            {
                if (this.MANDATO_ESP_MAN_NOM_VALUE == "" || this.MANDATO_ESP_MAN_NOM_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.MANDATO_ESP_MAN_NOM_VALUE;
            }
        }
        public string MANDATO_ESP_MAN_NOM_VALID { get; set; }


        public string MANDATO_ESP_REPRE_RUT_VALUE { get; set; }
        public string MANDATO_ESP_REPRE_RUT_VALUE_
        {
            get
            {
                if (this.MANDATO_ESP_REPRE_RUT_VALUE == "" || this.MANDATO_ESP_REPRE_RUT_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.MANDATO_ESP_REPRE_RUT_VALUE.Replace("-", "");
            }
        }
        public string MANDATO_ESP_REPRE_RUT_VALID { get; set; }


        public string MANDATO_ESP_PERSO_FECHA_VALUE { get; set; }
        public string MANDATO_ESP_PERSO_FECHA_VALUE_
        {
            get
            {
                if (this.MANDATO_ESP_PERSO_FECHA_VALUE == "" || this.MANDATO_ESP_PERSO_FECHA_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.MANDATO_ESP_PERSO_FECHA_VALUE;
            }
        }
        public string MANDATO_ESP_PERSO_FECHA_VALID { get; set; }


        public string MANDATO_ESP_NOTA_PERSONERIA_VALUE { get; set; }
        public string MANDATO_ESP_NOTA_PERSONERIA_VALUE_
        {
            get
            {
                if (this.MANDATO_ESP_NOTA_PERSONERIA_VALUE == "" || this.MANDATO_ESP_NOTA_PERSONERIA_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.MANDATO_ESP_NOTA_PERSONERIA_VALUE;
            }
        }
        public string MANDATO_ESP_NOTA_PERSONERIA_VALID { get; set; }


        public string MANDATO_ESP_REPRE_NOM_VALUE { get; set; }
        public string MANDATO_ESP_REPRE_NOM_VALUE_
        {
            get
            {
                if (this.MANDATO_ESP_REPRE_NOM_VALUE == "" || this.MANDATO_ESP_REPRE_NOM_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.MANDATO_ESP_REPRE_NOM_VALUE;
            }
        }
        public string MANDATO_ESP_REPRE_NOM_VALID { get; set; }


        public string MANDATO_FECHA_FIRMA_DEUDOR_VALUE { get; set; }
        public string MANDATO_FECHA_FIRMA_DEUDOR_VALUE_
        {

            get
            {
                if (this.MANDATO_FECHA_FIRMA_DEUDOR_VALUE == "" || this.MANDATO_FECHA_FIRMA_DEUDOR_VALUE is null)
                {
                    return Constant.TypeDate;

                }
                else return Convert.ToDateTime(this.MANDATO_FECHA_FIRMA_DEUDOR_VALUE).ToString("yyyy-MM-dd");
            }
        }
        public string MANDATO_FECHA_FIRMA_DEUDOR_VALID { get; set; }


        public string MANDATO_LUGAR_FIRMA_DEUDOR_VALUE { get; set; }
        public string MANDATO_LUGAR_FIRMA_DEUDOR_VALUE_
        {
            get
            {
                if (this.MANDATO_LUGAR_FIRMA_DEUDOR_VALUE == "" || this.MANDATO_LUGAR_FIRMA_DEUDOR_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.MANDATO_LUGAR_FIRMA_DEUDOR_VALUE;
            }
        }
        public string MANDATO_LUGAR_FIRMA_DEUDOR_VALID { get; set; }


        #endregion


        #region CONTRATOCREDITO
        public string CTOCRED_RUT_DEUDOR_VALUE { get; set; }
        public string CTOCRED_RUT_DEUDOR_VALUE_
        {
            get
            {
                if (this.CTOCRED_RUT_DEUDOR_VALUE == "" || this.CTOCRED_RUT_DEUDOR_VALUE is null)
                {
                    return Constant.stringZero;

                }
                else return this.CTOCRED_RUT_DEUDOR_VALUE.Replace("-", "");
            }
        }
        public string CTOCRED_RUT_DEUDOR_VALID { get; set; }


        public string CTOCRED_NOM_DEUDOR_VALUE { get; set; }
        public string CTOCRED_NOM_DEUDOR_VALUE_
        {
            get
            {
                if (this.CTOCRED_NOM_DEUDOR_VALUE == "" || this.CTOCRED_NOM_DEUDOR_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.CTOCRED_NOM_DEUDOR_VALUE;
            }
        }
        public string CTOCRED_NOM_DEUDOR_VALID { get; set; }


        public string CTOCRED_NOM_CODEUDOR_VALUE { get; set; }
        public string CTOCRED_NOM_CODEUDOR_VALUE_
        {
            get
            {
                if (this.CTOCRED_NOM_CODEUDOR_VALUE == "" || this.CTOCRED_NOM_CODEUDOR_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.CTOCRED_NOM_CODEUDOR_VALUE;
            }
        }
        public string CTOCRED_NOM_CODEUDOR_VALID { get; set; }


        public string CTOCRED_RUT_CODEUDOR_VALUE { get; set; }
        public string CTOCRED_RUT_CODEUDOR_VALUE_
        {
            get
            {
                if (this.CTOCRED_RUT_CODEUDOR_VALUE == "" || this.CTOCRED_RUT_CODEUDOR_VALUE is null)
                {
                    return Constant.stringZero;

                }
                else return this.CTOCRED_RUT_CODEUDOR_VALUE.Replace("-", "");
            }
        }
        public string CTOCRED_RUT_CODEUDOR_VALID { get; set; }


        public string CTOCRED_RUT_REP_LEGAL_VALUE { get; set; }
        public string CTOCRED_RUT_REP_LEGAL_VALUE_
        {
            get
            {
                if (this.CTOCRED_RUT_REP_LEGAL_VALUE == "" || this.CTOCRED_RUT_REP_LEGAL_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.CTOCRED_RUT_REP_LEGAL_VALUE.Replace("-", "");
            }
        }
        public string CTOCRED_RUT_REP_LEGAL_VALID { get; set; }


        public string CTOCRED_NOM_REP_LEGAL_VALUE { get; set; }
        public string CTOCRED_NOM_REP_LEGAL_VALUE_
        {
            get
            {
                if (this.CTOCRED_NOM_REP_LEGAL_VALUE == "" || this.CTOCRED_NOM_REP_LEGAL_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.CTOCRED_NOM_REP_LEGAL_VALUE;
            }
        }
        public string CTOCRED_NOM_REP_LEGAL_VALID { get; set; }


        public string CTOCRED_FEC_PERSONERIA_VALUE { get; set; }
        public string CTOCRED_FEC_PERSONERIA_VALUE_
        {
            get
            {
                if (this.CTOCRED_FEC_PERSONERIA_VALUE == "" || this.CTOCRED_FEC_PERSONERIA_VALUE is null || this.CTOCRED_FEC_PERSONERIA_VALUE == Constant.NoAplica)
                {
                    return Constant.TypeDate;
                }
                else return Convert.ToDateTime(this.CTOCRED_FEC_PERSONERIA_VALUE).ToString("yyyy-MM-dd");
            }
        }
        public string CTOCRED_FEC_PERSONERIA_VALID { get; set; }


        public string CTOCRED_NOTARIO_PERSONERIA_VALUE { get; set; }
        public string CTOCRED_NOTARIO_PERSONERIA_VALUE_
        {
            get
            {
                if (this.CTOCRED_NOTARIO_PERSONERIA_VALUE == "" || this.CTOCRED_NOTARIO_PERSONERIA_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.CTOCRED_NOTARIO_PERSONERIA_VALUE;
            }
        }
        public string CTOCRED_NOTARIO_PERSONERIA_VALID { get; set; }


        public string CTOCRED_TIPO_VALUE { get; set; }
        public string CTOCRED_TIPO_VALUE_
        {
            get
            {
                if (this.CTOCRED_TIPO_VALUE == "" || this.CTOCRED_TIPO_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.CTOCRED_TIPO_VALUE;
            }
        }
        public string CTOCRED_TIPO_VALID { get; set; }


        public string CTOCRED_MARCA_VALUE { get; set; }
        public string CTOCRED_MARCA_VALUE_
        {
            get
            {
                if (this.CTOCRED_MARCA_VALUE == "" || this.CTOCRED_MARCA_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.CTOCRED_MARCA_VALUE;
            }
        }
        public string CTOCRED_MARCA_VALID { get; set; }


        public string CTOCRED_MODELO_VALUE { get; set; }
        public string CTOCRED_MODELO_VALUE_
        {
            get
            {
                if (this.CTOCRED_MODELO_VALUE == "" || this.CTOCRED_MODELO_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.CTOCRED_MODELO_VALUE;
            }
        }
        public string CTOCRED_MODELO_VALID { get; set; }


        public string CTOCRED_ANIO_COMERCIAL_VALUE { get; set; }
        public string CTOCRED_ANIO_COMERCIAL_VALUE_
        {
            get
            {
                if (this.CTOCRED_ANIO_COMERCIAL_VALUE == "" || this.CTOCRED_ANIO_COMERCIAL_VALUE is null)
                {
                    return Constant.stringZero;

                }
                else return this.CTOCRED_ANIO_COMERCIAL_VALUE;
            }
        }
        public string CTOCRED_ANIO_COMERCIAL_VALID { get; set; }


        public string CTOCRED_VALOR_COMERCIAL_VALUE { get; set; }
        public string CTOCRED_VALOR_COMERCIAL_VALUE_
        {
            get
            {
                if (this.CTOCRED_VALOR_COMERCIAL_VALUE == "" || this.CTOCRED_VALOR_COMERCIAL_VALUE is null)
                {
                    return Constant.stringZero;

                }
                else return this.CTOCRED_VALOR_COMERCIAL_VALUE;
            }
        }
        public string CTOCRED_VALOR_COMERCIAL_VALID { get; set; }


        public string CTOCRED_VALOR_VEHI_VALUE { get; set; }
        public string CTOCRED_VALOR_VEHI_VALUE_
        {
            get
            {
                if (this.CTOCRED_VALOR_VEHI_VALUE == "" || this.CTOCRED_VALOR_VEHI_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.CTOCRED_VALOR_VEHI_VALUE;
            }
        }
        public string CTOCRED_VALOR_VEHI_VALID { get; set; }


        public string CTOCRED_PIE_VALUE { get; set; }
        public string CTOCRED_PIE_VALUE_
        {
            get
            {
                if (this.CTOCRED_PIE_VALUE == "" || this.CTOCRED_PIE_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.CTOCRED_PIE_VALUE;
            }
        }
        public string CTOCRED_PIE_VALID { get; set; }


        public string CTOCRED_TOTAL_VALUE { get; set; }
        public string CTOCRED_TOTAL_VALUE_
        {
            get
            {
                if (this.CTOCRED_TOTAL_VALUE == "" || this.CTOCRED_TOTAL_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.CTOCRED_TOTAL_VALUE;
            }
        }
        public string CTOCRED_TOTAL_VALID { get; set; }


        public string CTOCRED_NUM_CUOTA_VALUE { get; set; }
        public string CTOCRED_NUM_CUOTA_VALUE_
        {
            get
            {
                if (this.CTOCRED_NUM_CUOTA_VALUE == "" || this.CTOCRED_NUM_CUOTA_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.CTOCRED_NUM_CUOTA_VALUE;
            }
        }
        public string CTOCRED_NUM_CUOTA_VALID { get; set; }


        public string CTOCRED_VALOR_CUOTA_VALUE { get; set; }
        public string CTOCRED_VALOR_CUOTA_VALUE_
        {
            get
            {
                if (this.CTOCRED_VALOR_CUOTA_VALUE == "" || this.CTOCRED_VALOR_CUOTA_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.CTOCRED_VALOR_CUOTA_VALUE;
            }
        }
        public string CTOCRED_VALOR_CUOTA_VALID { get; set; }


        public string CTOCRED_DIAS_CUOT_VALUE { get; set; }
        public string CTOCRED_DIAS_CUOT_VALUE_
        {
            get
            {
                if (this.CTOCRED_DIAS_CUOT_VALUE == "" || this.CTOCRED_DIAS_CUOT_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.CTOCRED_DIAS_CUOT_VALUE;
            }
        }
        public string CTOCRED_DIAS_CUOT_VALID { get; set; }


        public string CTOCRED_FECHA_CUOT_VALUE { get; set; }
        public string CTOCRED_FECHA_CUOT_VALUE_
        {
            get
            {
                if (this.CTOCRED_FECHA_CUOT_VALUE == "" || this.CTOCRED_FECHA_CUOT_VALUE is null)
                {
                    return Constant.TypeDate;

                }
                else return Convert.ToDateTime(this.CTOCRED_FECHA_CUOT_VALUE).ToString("yyyy-MM-dd");
            }
        }
        public string CTOCRED_FECHA_CUOT_VALID { get; set; }


        public string CTOCRED_NUM_CUOTA_VARIABLE_VALUE { get; set; }
        public string CTOCRED_NUM_CUOTA_VARIABLE_VALUE_
        {
            get
            {
                if (this.CTOCRED_NUM_CUOTA_VARIABLE_VALUE == "" || this.CTOCRED_NUM_CUOTA_VARIABLE_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.CTOCRED_NUM_CUOTA_VARIABLE_VALUE;
            }
        }
        public string CTOCRED_NUM_CUOTA_VARIABLE_VALID { get; set; }


        public string CTOCRED_SERVI_PREN_VALUE { get; set; }
        public string CTOCRED_SERVI_PREN_VALUE_
        {
            get
            {
                if (this.CTOCRED_SERVI_PREN_VALUE == "" || this.CTOCRED_SERVI_PREN_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.CTOCRED_SERVI_PREN_VALUE;
            }
        }
        public string CTOCRED_SERVI_PREN_VALID { get; set; }


        public string CTOCRED_INTERES_DESFACE_VALUE { get; set; }
        public string CTOCRED_INTERES_DESFACE_VALUE_
        {
            get
            {
                if (this.CTOCRED_INTERES_DESFACE_VALUE == "" || this.CTOCRED_INTERES_DESFACE_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.CTOCRED_INTERES_DESFACE_VALUE;
            }
        }
        public string CTOCRED_INTERES_DESFACE_VALID { get; set; }


        public string CTOCRED_IMPU_TIMB_VALUE { get; set; }
        public string CTOCRED_IMPU_TIMB_VALUE_
        {
            get
            {
                if (this.CTOCRED_IMPU_TIMB_VALUE == "" || this.CTOCRED_IMPU_TIMB_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.CTOCRED_IMPU_TIMB_VALUE;
            }
        }
        public string CTOCRED_IMPU_TIMB_VALID { get; set; }


        public string CTOCRED_GASTOS_MANTEN_VALUE { get; set; }
        public string CTOCRED_GASTOS_MANTEN_VALUE_
        {
            get
            {
                if (this.CTOCRED_GASTOS_MANTEN_VALUE == "" || this.CTOCRED_GASTOS_MANTEN_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.CTOCRED_GASTOS_MANTEN_VALUE;
            }
        }
        public string CTOCRED_GASTOS_MANTEN_VALID { get; set; }


        public string CTOCRED_CLUB_TAXI_VALUE { get; set; }
        public string CTOCRED_CLUB_TAXI_VALUE_
        {
            get
            {
                if (this.CTOCRED_CLUB_TAXI_VALUE == "" || this.CTOCRED_CLUB_TAXI_VALUE is null)
                {
                    return Constant.stringZero;

                }
                else return this.CTOCRED_CLUB_TAXI_VALUE;
            }
        }
        public string CTOCRED_CLUB_TAXI_VALID { get; set; }


        public string CTOCRED_SEGURO_DESG_VALUE { get; set; }
        public string CTOCRED_SEGURO_DESG_VALUE_
        {
            get
            {
                if (this.CTOCRED_SEGURO_DESG_VALUE == "" || this.CTOCRED_SEGURO_DESG_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.CTOCRED_SEGURO_DESG_VALUE;
            }
        }
        public string CTOCRED_SEGURO_DESG_VALID { get; set; }


        public string CTOCRED_SEG_DOBLE_CAPITAL_VALUE { get; set; }
        public string CTOCRED_SEG_DOBLE_CAPITAL_VALUE_
        {
            get
            {
                if (this.CTOCRED_SEG_DOBLE_CAPITAL_VALUE == "" || this.CTOCRED_SEG_DOBLE_CAPITAL_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.CTOCRED_SEG_DOBLE_CAPITAL_VALUE;
            }
        }
        public string CTOCRED_SEG_DOBLE_CAPITAL_VALID { get; set; }


        public string CTOCRED_SEGURO_DESE_VALUE { get; set; }
        public string CTOCRED_SEGURO_DESE_VALUE_
        {
            get
            {
                if (this.CTOCRED_SEGURO_DESE_VALUE == "" || this.CTOCRED_SEGURO_DESE_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.CTOCRED_SEGURO_DESE_VALUE;
            }
        }
        public string CTOCRED_SEGURO_DESE_VALID { get; set; }


        public string CTOCRED_SEGURO_AUTO_VALUE { get; set; }
        public string CTOCRED_SEGURO_AUTO_VALUE_
        {
            get
            {
                if (this.CTOCRED_SEGURO_AUTO_VALUE == "" || this.CTOCRED_SEGURO_AUTO_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.CTOCRED_SEGURO_AUTO_VALUE;
            }
        }
        public string CTOCRED_SEGURO_AUTO_VALID { get; set; }


        public string CTOCRED_FIRMA_DEUDOR_VALUE { get; set; }
        public string CTOCRED_FIRMA_DEUDOR_VALUE_
        {
            get
            {
                if (this.CTOCRED_FIRMA_DEUDOR_VALUE == "" || this.CTOCRED_FIRMA_DEUDOR_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.CTOCRED_FIRMA_DEUDOR_VALUE;
            }
        }
        public string CTOCRED_FIRMA_DEUDOR_VALID { get; set; }


        public string CTOCRED_SERV_EXTERNO_VALUE { get; set; }
        public string CTOCRED_SERV_EXTERNO_VALUE_
        {
            get
            {
                if (this.CTOCRED_SERV_EXTERNO_VALUE == "" || this.CTOCRED_SERV_EXTERNO_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.CTOCRED_SERV_EXTERNO_VALUE;
            }
        }
        public string CTOCRED_SERV_EXTERNO_VALID { get; set; }
        #endregion


        #region SOLPRIM
        public string SOL_PRIM_NUM_FAC_VALUE { get; set; }
        public string SOL_PRIM_NUM_FAC_VALUE_
        {
            get
            {
                if (this.SOL_PRIM_NUM_FAC_VALUE == "" || this.SOL_PRIM_NUM_FAC_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.SOL_PRIM_NUM_FAC_VALUE;
            }
        }
        public string SOL_PRIM_NUM_FAC_VALID { get; set; }


        public string SOL_PRIM_NOM_ADQUI_VALUE { get; set; }
        public string SOL_PRIM_NOM_ADQUI_VALUE_
        {
            get
            {
                if (this.SOL_PRIM_NOM_ADQUI_VALUE == "" || this.SOL_PRIM_NOM_ADQUI_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.SOL_PRIM_NOM_ADQUI_VALUE;
            }
        }
        public string SOL_PRIM_NOM_ADQUI_VALID { get; set; }


        public string SOL_PRIM_RUT_ADQUI_VALUE { get; set; }
        public string SOL_PRIM_RUT_ADQUI_VALUE_
        {
            get
            {
                if (this.SOL_PRIM_RUT_ADQUI_VALUE == "" || this.SOL_PRIM_RUT_ADQUI_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.SOL_PRIM_RUT_ADQUI_VALUE.Replace("-", "");
            }
        }
        public string SOL_PRIM_RUT_ADQUI_VALID { get; set; }


        public string SOL_PRIM_COLOR_VALUE { get; set; }
        public string SOL_PRIM_COLOR_VALUE_
        {
            get
            {
                if (this.SOL_PRIM_COLOR_VALUE == "" || this.SOL_PRIM_COLOR_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.SOL_PRIM_COLOR_VALUE;
            }
        }
        public string SOL_PRIM_COLOR_VALID { get; set; }


        public string SOL_PRIM_PATENTE_VALUE { get; set; }
        public string SOL_PRIM_PATENTE_VALUE_
        {
            get
            {
                if (this.SOL_PRIM_PATENTE_VALUE == "" || this.SOL_PRIM_PATENTE_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.SOL_PRIM_PATENTE_VALUE;
            }
        }
        public string SOL_PRIM_PATENTE_VALID { get; set; }


        public string SOL_PRIM_MARCA_VALUE { get; set; }
        public string SOL_PRIM_MARCA_VALUE_
        {
            get
            {
                if (this.SOL_PRIM_MARCA_VALUE == "" || this.SOL_PRIM_MARCA_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.SOL_PRIM_MARCA_VALUE;
            }
        }
        public string SOL_PRIM_MARCA_VALID { get; set; }


        public string SOL_PRIM_MODELO_VALUE { get; set; }
        public string SOL_PRIM_MODELO_VALUE_
        {
            get
            {
                if (this.SOL_PRIM_MODELO_VALUE == "" || this.SOL_PRIM_MODELO_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.SOL_PRIM_MODELO_VALUE;
            }
        }
        public string SOL_PRIM_MODELO_VALID { get; set; }


        public string SOL_PRIM_ANIO_VALUE { get; set; }
        public string SOL_PRIM_ANIO_VALUE_
        {
            get
            {
                if (this.SOL_PRIM_ANIO_VALUE == "" || this.SOL_PRIM_ANIO_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.SOL_PRIM_ANIO_VALUE;
            }
        }
        public string SOL_PRIM_ANIO_VALID { get; set; }


        public string SOL_PRIM_MOTOR_VALUE { get; set; }
        public string SOL_PRIM_MOTOR_VALUE_
        {
            get
            {
                if (this.SOL_PRIM_MOTOR_VALUE == "" || this.SOL_PRIM_MOTOR_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.SOL_PRIM_MOTOR_VALUE;
            }
        }
        public string SOL_PRIM_MOTOR_VALID { get; set; }


        public string SOL_PRIM_CHASIS_VALUE { get; set; }
        public string SOL_PRIM_CHASIS_VALUE_
        {
            get
            {
                if (this.SOL_PRIM_CHASIS_VALUE == "" || this.SOL_PRIM_CHASIS_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.SOL_PRIM_CHASIS_VALUE;
            }
        }
        public string SOL_PRIM_CHASIS_VALID { get; set; }
        #endregion


        #region FACTURA

        public string FAC_NUM_VALUE { get; set; }
        public string FAC_NUM_VALUE_
        {
            get
            {
                if (this.FAC_NUM_VALUE == "" || this.FAC_NUM_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.FAC_NUM_VALUE;
            }
        }
        public string FAC_NUM_VALID { get; set; }


        public string FAC_RUT_CONCE_VALUE { get; set; }
        public string FAC_RUT_CONCE_VALUE_
        {
            get
            {
                if (this.FAC_RUT_CONCE_VALUE == "" || this.FAC_RUT_CONCE_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.FAC_RUT_CONCE_VALUE.Replace("-", ""); ;
            }
        }
        public string FAC_RUT_CONCE_VALID { get; set; }


        public string FAC_RUT_CLIEN_VALUE { get; set; }
        public string FAC_RUT_CLIEN_VALUE_
        {
            get
            {
                if (this.FAC_RUT_CLIEN_VALUE == "" || this.FAC_RUT_CLIEN_VALUE is null)
                {
                    return Constant.stringZero;

                }
                else return this.FAC_RUT_CLIEN_VALUE.Replace("-", ""); 
            }
        }
        public string FAC_RUT_CLIEN_VALID { get; set; }


        public string FAC_NOM_CLIEN_VALUE { get; set; }
        public string FAC_NOM_CLIEN_VALUE_
        {
            get
            {
                if (this.FAC_NOM_CLIEN_VALUE == "" || this.FAC_NOM_CLIEN_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.FAC_NOM_CLIEN_VALUE;
            }
        }
        public string FAC_NOM_CLIEN_VALID { get; set; }


        public string FAC_RUT_COMPRA_VALUE { get; set; }
        public string FAC_RUT_COMPRA_VALUE_
        {
            get
            {
                if (this.FAC_RUT_COMPRA_VALUE == "" || this.FAC_RUT_COMPRA_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.FAC_RUT_COMPRA_VALUE.Replace("-", "");
            }
        }
        public string FAC_RUT_COMPRA_VALID { get; set; }


        public string FAC_NOM_COMPRA_VALUE { get; set; }
        public string FAC_NOM_COMPRA_VALUE_
        {
            get
            {
                if (this.FAC_NOM_COMPRA_VALUE == "" || this.FAC_NOM_COMPRA_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.FAC_NOM_COMPRA_VALUE;
            }
        }
        public string FAC_NOM_COMPRA_VALID { get; set; }


        public string FAC_MARCA_VALUE { get; set; }
        public string FAC_MARCA_VALUE_
        {
            get
            {
                if (this.FAC_MARCA_VALUE == "" || this.FAC_MARCA_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.FAC_MARCA_VALUE;
            }
        }
        public string FAC_MARCA_VALID { get; set; }


        public string FAC_MODELO_VALUE { get; set; }
        public string FAC_MODELO_VALUE_
        {
            get
            {
                if (this.FAC_MODELO_VALUE == "" || this.FAC_MODELO_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.FAC_MODELO_VALUE;
            }
        }
        public string FAC_MODELO_VALID { get; set; }


        public string FAC_CHASIS_VALUE { get; set; }
        public string FAC_CHASIS_VALUE_
        {
            get
            {
                if (this.FAC_CHASIS_VALUE == "" || this.FAC_CHASIS_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.FAC_CHASIS_VALUE;
            }
        }
        public string FAC_CHASIS_VALID { get; set; }


        public string FAC_CAJON_VALUE { get; set; }
        public string FAC_CAJON_VALUE_
        {
            get
            {
                if (this.FAC_CAJON_VALUE == "" || this.FAC_CAJON_VALUE is null)
                {
                    return Constant.TypeText;
                }
                else return this.FAC_CAJON_VALUE;
            }
        }
        public string FAC_CAJON_VALID { get; set; }


        public string FAC_ANIO_COMERCIAL_VALUE { get; set; }
        public string FAC_ANIO_COMERCIAL_VALUE_
        {
            get
            {
                if (this.FAC_ANIO_COMERCIAL_VALUE == "" || this.FAC_ANIO_COMERCIAL_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.FAC_ANIO_COMERCIAL_VALUE;
            }
        }
        public string FAC_ANIO_COMERCIAL_VALID { get; set; }


        public string FAC_MONTO_TOTAL_VALUE { get; set; }
        public string FAC_MONTO_TOTAL_VALUE_
        {
            get
            {
                if (this.FAC_MONTO_TOTAL_VALUE == "" || this.FAC_MONTO_TOTAL_VALUE is null)
                {
                    return Constant.stringZero;
                }
                else return this.FAC_MONTO_TOTAL_VALUE;
            }
        }
        public string FAC_MONTO_TOTAL_VALID { get; set; }


        public string FAC_MOTOR_VALUE { get; set; }
        public string FAC_MOTOR_VALUE_
        {
            get
            {
                if (this.FAC_MOTOR_VALUE == "" || this.FAC_MOTOR_VALUE is null)
                {
                    return Constant.TypeText;

                }
                else return this.FAC_MOTOR_VALUE;
            }
        }
        public string FAC_MOTOR_VALID { get; set; }
        #endregion


        #region CAV
        public string CAV_RUT_CLIENTE_VALUE { get; set; }
        public string CAV_RUT_CLIENTE_VALID { get; set; }
        public string CAV_NOM_PROPIETARIO_VALUE { get; set; }
        public string CAV_NOM_PROPIETARIO_VALID { get; set; }
        public string CAV_FOLIO_DOCUMENTO_VALUE { get; set; }
        public string CAV_FOLIO_DOCUMENTO_VALID { get; set; }
        public string CAV_COD_VERIFICACION_VALUE { get; set; }
        public string CAV_COD_VERIFICACION_VALID { get; set; }
        public string CAV_PATENTE_VALUE { get; set; }
        public string CAV_PATENTE_VALID { get; set; }
        public string CAV_MARCA_VALUE { get; set; }
        public string CAV_MARCA_VALID { get; set; }
        public string CAV_MODELO_VALUE { get; set; }
        public string CAV_MODELO_VALID { get; set; }
        public string CAV_NUM_MOTOR_VALUE { get; set; }
        public string CAV_NUM_MOTOR_VALID { get; set; }
        public string CAV_NUM_CHASIS_VALUE { get; set; }
        public string CAV_NUM_CHASIS_VALID { get; set; }
        public string CAV_COLOR_VALUE { get; set; }
        public string CAV_COLOR_VALID { get; set; }
        public string CAV_ANIO_VALUE { get; set; }
        public string CAV_ANIO_VALID { get; set; }
        #endregion|

    }
}
