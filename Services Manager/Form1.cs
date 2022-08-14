using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.ServiceProcess;
using System.Management;
using System.Dynamic;

namespace Services_Manager
{
    public partial class Form1 : Form
    {
        
        uint processId = 0,sid=0;
        dynamic rm;
        public Form1()
        {
            InitializeComponent();
        }
        //EVENTOS PARA OBTENER LOS VALORES DE LOS DATAGRIDVIEW EN LAS PESTAÑAS DE PROCESOS Y SERVICIOS
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //SELECCIONANDO EL ÍNDICE Y PASANDO LO VALORES DE LOS PROCESOS
            int get = e.RowIndex;
            DataGridViewRow rows = dataGridView1.Rows[get];
            namTB.Text = rows.Cells[0].Value.ToString();
            pibTB.Text = rows.Cells[1].Value.ToString();
            cpuTB.Text = rows.Cells[2].Value.ToString();
            ramTB.Text = rows.Cells[3].Value.ToString();
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //SELECCIONANDO EL ÍNDICE Y PASANDO LO VALORES DE LOS SERVICIOS
            int get = e.RowIndex;
            DataGridViewRow rows = dataGridView2.Rows[get];
            snamTB.Text = rows.Cells[0].Value.ToString();
            sidTB.Text = rows.Cells[1].Value.ToString();
            scpuTB.Text = rows.Cells[2].Value.ToString();
            sramTB.Text = rows.Cells[3].Value.ToString();
        }

        //OBTENIENDO EL PID DE CADA SERVICIO CON OBJECTSEARCHER (RETORNA UN UINT CON EL VALOR DEL PID DEL SERVICIO ACTUAL)
        uint GetProcessIDByServiceName(string serviceName)
        {
            
            string qry = "SELECT PROCESSID FROM WIN32_SERVICE WHERE NAME = '" + serviceName + "'";
            System.Management.ManagementObjectSearcher searcher = new System.Management.ManagementObjectSearcher(qry);
            foreach (System.Management.ManagementObject mngntObj in searcher.Get())
            {
                processId = (uint)mngntObj["PROCESSID"];
            }
            return processId;
        }

     

        //MOSTRANDO LA INFORMACIÓN SOLICITADA
        private void Form1_Shown(object sender, EventArgs e)
        {
            //PRIMERA TAREA (BUSCA LOS PROCESOS Y SU INFORMACIÓN Y LA MUESTRA EN EL DATAGRIDVIEW DE PROCESOS)
            var task1 = new Task(() =>
            {
                Thread.Sleep(0);
                CheckForIllegalCrossThreadCalls = false; //PARA PODER ATRAPAR LAS ASIGNACIONES ERRONEAS DEL CONTROL AL THREAD
                Process[] proc = Process.GetProcesses();

                //RECORRIENDO LOS PROCESOS, SACANDO LA INFORMACIÓN Y AÑADIENDOLA AL DATAGRIDVIEW PROCESOS
            foreach (Process p in proc)
            {
                PerformanceCounter percentP = new PerformanceCounter("Process", "% Processor Time", p.ProcessName, true);
                percentP.NextValue();

                dataGridView1.Rows.Add(p.ProcessName, p.Id,(( Math.Round( percentP.NextValue(),0))/100 * .100)*100+"%"/*REDONDEANDO EL PORCENTAJE CPU*/, p.WorkingSet64 / 1048576+"MB");
                   
                }
               

            });
            //INICIANDO TAREA 1
            task1.Start();

            //SEGUNDA TAREA (BUSCA LOS SERVICIOS Y SU INFORMACIÓN Y LA MUESTRA EN EL DATAGRIDVIEW DE PROCESOS)
            var task2 = new Task(() =>
            {
                Thread.Sleep(0);
                CheckForIllegalCrossThreadCalls = false;
                ServiceController[] serv = ServiceController.GetServices();

                //RECORRIENDO LOS SERVICIOS, SACANDO LA INFORMACIÓN Y AÑADIENDOLA AL DATAGRIDVIEW SERVICIOS
                foreach (ServiceController s in serv)
                {
                    sid = GetProcessIDByServiceName(s.ServiceName);//OBTENIENDO EL PID DEL SERVICIO
                    dataGridView2.Rows.Add(s.ServiceName,sid,s.ServiceType, s.Status);

                }

            });
            //INICIANDO TAREA 2
            task2.Start();
        }
    }
}
