using Entidad_BE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Servicios
{
    public class VerificadorIntegridad
    {
        public static string CalcularDVH(IVerificable entidad)
        {
            string datos = entidad.ObtenerCamposDV();
            return Encriptador.EncriptarIrrev(datos);
        }

        public static string CalcularDVV(List<string>dvh)
        {
            string aux = string.Join("|", dvh);
            return Encriptador.EncriptarIrrev(aux);
        }
    }
}
