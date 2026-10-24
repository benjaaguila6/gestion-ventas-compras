namespace Servicios
{
    partial class RecepcionProductos
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblInstruccion = new System.Windows.Forms.Label();
            this.gbOrdenes = new System.Windows.Forms.GroupBox();
            this.dgvOrdenes = new System.Windows.Forms.DataGridView();
            this.gbProductos = new System.Windows.Forms.GroupBox();
            this.dgvProductos = new System.Windows.Forms.DataGridView();
            this.btnModificarEntrega = new System.Windows.Forms.Button();
            this.btnRegistrarRecepcion = new System.Windows.Forms.Button();
            this.gbOrdenes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).BeginInit();
            this.gbProductos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).BeginInit();
            this.SuspendLayout();
            // 
            // lblInstruccion
            // 
            this.lblInstruccion.AutoSize = true;
            this.lblInstruccion.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstruccion.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblInstruccion.Location = new System.Drawing.Point(38, 27);
            this.lblInstruccion.Name = "lblInstruccion";
            this.lblInstruccion.Size = new System.Drawing.Size(1060, 38);
            this.lblInstruccion.TabIndex = 0;
            this.lblInstruccion.Text = "Seleccione una orden de compra para registrar el ingreso:";
            // 
            // gbOrdenes
            // 
            this.gbOrdenes.Controls.Add(this.dgvOrdenes);
            this.gbOrdenes.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbOrdenes.Location = new System.Drawing.Point(29, 80);
            this.gbOrdenes.Name = "gbOrdenes";
            this.gbOrdenes.Size = new System.Drawing.Size(1254, 300);
            this.gbOrdenes.TabIndex = 1;
            this.gbOrdenes.TabStop = false;
            this.gbOrdenes.Text = "Ordenes de compra";
            // 
            // dgvOrdenes
            // 
            this.dgvOrdenes.AllowUserToAddRows = false;
            this.dgvOrdenes.AllowUserToDeleteRows = false;
            this.dgvOrdenes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvOrdenes.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvOrdenes.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrdenes.Location = new System.Drawing.Point(16, 40);
            this.dgvOrdenes.MultiSelect = false;
            this.dgvOrdenes.Name = "dgvOrdenes";
            this.dgvOrdenes.ReadOnly = true;
            this.dgvOrdenes.RowHeadersWidth = 82;
            this.dgvOrdenes.RowTemplate.Height = 33;
            this.dgvOrdenes.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOrdenes.Size = new System.Drawing.Size(1220, 245);
            this.dgvOrdenes.TabIndex = 0;
            this.dgvOrdenes.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvOrdenes_CellFormatting);
            this.dgvOrdenes.SelectionChanged += new System.EventHandler(this.dgvOrdenes_SelectionChanged);
            // 
            // gbProductos
            // 
            this.gbProductos.Controls.Add(this.dgvProductos);
            this.gbProductos.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbProductos.Location = new System.Drawing.Point(29, 400);
            this.gbProductos.Name = "gbProductos";
            this.gbProductos.Size = new System.Drawing.Size(1254, 580);
            this.gbProductos.TabIndex = 2;
            this.gbProductos.TabStop = false;
            this.gbProductos.Text = "Productos de la orden";
            // 
            // dgvProductos
            // 
            this.dgvProductos.AllowUserToAddRows = false;
            this.dgvProductos.AllowUserToDeleteRows = false;
            this.dgvProductos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvProductos.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvProductos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProductos.Location = new System.Drawing.Point(16, 40);
            this.dgvProductos.MultiSelect = false;
            this.dgvProductos.Name = "dgvProductos";
            this.dgvProductos.RowHeadersWidth = 82;
            this.dgvProductos.RowTemplate.Height = 33;
            this.dgvProductos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProductos.Size = new System.Drawing.Size(1220, 520);
            this.dgvProductos.TabIndex = 0;
            this.dgvProductos.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvProductos_DataError);
            this.dgvProductos.SelectionChanged += new System.EventHandler(this.dgvProductos_SelectionChanged);
            // 
            // btnModificarEntrega
            // 
            this.btnModificarEntrega.BackColor = System.Drawing.Color.SteelBlue;
            this.btnModificarEntrega.Enabled = false;
            this.btnModificarEntrega.Font = new System.Drawing.Font("Verdana", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnModificarEntrega.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnModificarEntrega.Location = new System.Drawing.Point(1324, 350);
            this.btnModificarEntrega.Margin = new System.Windows.Forms.Padding(4);
            this.btnModificarEntrega.Name = "btnModificarEntrega";
            this.btnModificarEntrega.Size = new System.Drawing.Size(340, 153);
            this.btnModificarEntrega.TabIndex = 3;
            this.btnModificarEntrega.Text = "MODIFICAR CANT. ENTREGA";
            this.btnModificarEntrega.UseVisualStyleBackColor = false;
            this.btnModificarEntrega.Click += new System.EventHandler(this.btnModificarEntrega_Click);
            // 
            // btnRegistrarRecepcion
            // 
            this.btnRegistrarRecepcion.BackColor = System.Drawing.Color.SteelBlue;
            this.btnRegistrarRecepcion.Enabled = false;
            this.btnRegistrarRecepcion.Font = new System.Drawing.Font("Verdana", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRegistrarRecepcion.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnRegistrarRecepcion.Location = new System.Drawing.Point(1324, 511);
            this.btnRegistrarRecepcion.Margin = new System.Windows.Forms.Padding(4);
            this.btnRegistrarRecepcion.Name = "btnRegistrarRecepcion";
            this.btnRegistrarRecepcion.Size = new System.Drawing.Size(340, 133);
            this.btnRegistrarRecepcion.TabIndex = 4;
            this.btnRegistrarRecepcion.Text = "REGISTRAR RECEPCION";
            this.btnRegistrarRecepcion.UseVisualStyleBackColor = false;
            this.btnRegistrarRecepcion.Click += new System.EventHandler(this.btnRegistrarRecepcion_Click);
            // 
            // RecepcionProductos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(1717, 1040);
            this.Controls.Add(this.btnRegistrarRecepcion);
            this.Controls.Add(this.btnModificarEntrega);
            this.Controls.Add(this.gbProductos);
            this.Controls.Add(this.gbOrdenes);
            this.Controls.Add(this.lblInstruccion);
            this.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.Name = "RecepcionProductos";
            this.Text = "RecepcionProductos";
            this.gbOrdenes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrdenes)).EndInit();
            this.gbProductos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProductos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInstruccion;
        private System.Windows.Forms.GroupBox gbOrdenes;
        private System.Windows.Forms.DataGridView dgvOrdenes;
        private System.Windows.Forms.GroupBox gbProductos;
        private System.Windows.Forms.DataGridView dgvProductos;
        private System.Windows.Forms.Button btnModificarEntrega;
        private System.Windows.Forms.Button btnRegistrarRecepcion;
    }
}
