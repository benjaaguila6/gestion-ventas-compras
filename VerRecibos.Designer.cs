namespace Servicios
{
    partial class VerRecibos
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
            this.gbFiltros = new System.Windows.Forms.GroupBox();
            this.lblNumeroOrden = new System.Windows.Forms.Label();
            this.txtNumeroOrden = new System.Windows.Forms.TextBox();
            this.lblProveedor = new System.Windows.Forms.Label();
            this.cboProveedor = new System.Windows.Forms.ComboBox();
            this.chkUsarFecha = new System.Windows.Forms.CheckBox();
            this.lblEstado = new System.Windows.Forms.Label();
            this.cboEstado = new System.Windows.Forms.ComboBox();
            this.lblFechaDesde = new System.Windows.Forms.Label();
            this.dtpFechaDesde = new System.Windows.Forms.DateTimePicker();
            this.lblFechaHasta = new System.Windows.Forms.Label();
            this.dtpFechaHasta = new System.Windows.Forms.DateTimePicker();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.btnLimpiar = new System.Windows.Forms.Button();
            this.gbRecibos = new System.Windows.Forms.GroupBox();
            this.dgvRecibos = new System.Windows.Forms.DataGridView();
            this.btnImprimirRemito = new System.Windows.Forms.Button();
            this.gbFiltros.SuspendLayout();
            this.gbRecibos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecibos)).BeginInit();
            this.SuspendLayout();
            // 
            // lblInstruccion
            // 
            this.lblInstruccion.AutoSize = true;
            this.lblInstruccion.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInstruccion.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblInstruccion.Location = new System.Drawing.Point(29, 20);
            this.lblInstruccion.Name = "lblInstruccion";
            this.lblInstruccion.Size = new System.Drawing.Size(820, 38);
            this.lblInstruccion.TabIndex = 0;
            this.lblInstruccion.Text = "Órdenes de compra con mercadería recibida:";
            // 
            // gbFiltros
            // 
            this.gbFiltros.Controls.Add(this.lblNumeroOrden);
            this.gbFiltros.Controls.Add(this.txtNumeroOrden);
            this.gbFiltros.Controls.Add(this.lblProveedor);
            this.gbFiltros.Controls.Add(this.cboProveedor);
            this.gbFiltros.Controls.Add(this.chkUsarFecha);
            this.gbFiltros.Controls.Add(this.lblEstado);
            this.gbFiltros.Controls.Add(this.cboEstado);
            this.gbFiltros.Controls.Add(this.lblFechaDesde);
            this.gbFiltros.Controls.Add(this.dtpFechaDesde);
            this.gbFiltros.Controls.Add(this.lblFechaHasta);
            this.gbFiltros.Controls.Add(this.dtpFechaHasta);
            this.gbFiltros.Controls.Add(this.btnBuscar);
            this.gbFiltros.Controls.Add(this.btnLimpiar);
            this.gbFiltros.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbFiltros.Location = new System.Drawing.Point(29, 70);
            this.gbFiltros.Name = "gbFiltros";
            this.gbFiltros.Size = new System.Drawing.Size(1512, 161);
            this.gbFiltros.TabIndex = 1;
            this.gbFiltros.TabStop = false;
            this.gbFiltros.Text = "Filtros";
            // 
            // lblNumeroOrden
            // 
            this.lblNumeroOrden.AutoSize = true;
            this.lblNumeroOrden.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumeroOrden.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblNumeroOrden.Location = new System.Drawing.Point(6, 38);
            this.lblNumeroOrden.Name = "lblNumeroOrden";
            this.lblNumeroOrden.Size = new System.Drawing.Size(271, 29);
            this.lblNumeroOrden.TabIndex = 0;
            this.lblNumeroOrden.Text = "N° Orden de compra:";
            // 
            // txtNumeroOrden
            // 
            this.txtNumeroOrden.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNumeroOrden.Location = new System.Drawing.Point(283, 35);
            this.txtNumeroOrden.Name = "txtNumeroOrden";
            this.txtNumeroOrden.Size = new System.Drawing.Size(90, 37);
            this.txtNumeroOrden.TabIndex = 1;
            // 
            // lblProveedor
            // 
            this.lblProveedor.AutoSize = true;
            this.lblProveedor.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProveedor.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblProveedor.Location = new System.Drawing.Point(391, 38);
            this.lblProveedor.Name = "lblProveedor";
            this.lblProveedor.Size = new System.Drawing.Size(145, 29);
            this.lblProveedor.TabIndex = 2;
            this.lblProveedor.Text = "Proveedor:";
            // 
            // cboProveedor
            // 
            this.cboProveedor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProveedor.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboProveedor.FormattingEnabled = true;
            this.cboProveedor.Location = new System.Drawing.Point(542, 35);
            this.cboProveedor.Name = "cboProveedor";
            this.cboProveedor.Size = new System.Drawing.Size(270, 37);
            this.cboProveedor.TabIndex = 3;
            // 
            // chkUsarFecha
            // 
            this.chkUsarFecha.AutoSize = true;
            this.chkUsarFecha.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUsarFecha.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.chkUsarFecha.Location = new System.Drawing.Point(519, 114);
            this.chkUsarFecha.Name = "chkUsarFecha";
            this.chkUsarFecha.Size = new System.Drawing.Size(234, 33);
            this.chkUsarFecha.TabIndex = 4;
            this.chkUsarFecha.Text = "Filtrar por fecha";
            this.chkUsarFecha.UseVisualStyleBackColor = true;
            this.chkUsarFecha.CheckedChanged += new System.EventHandler(this.chkUsarFecha_CheckedChanged);
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstado.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblEstado.Location = new System.Drawing.Point(1076, 38);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(204, 29);
            this.lblEstado.TabIndex = 5;
            this.lblEstado.Text = "Estado entrega:";
            // 
            // cboEstado
            // 
            this.cboEstado.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEstado.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboEstado.FormattingEnabled = true;
            this.cboEstado.Location = new System.Drawing.Point(1292, 30);
            this.cboEstado.Name = "cboEstado";
            this.cboEstado.Size = new System.Drawing.Size(200, 37);
            this.cboEstado.TabIndex = 6;
            // 
            // lblFechaDesde
            // 
            this.lblFechaDesde.AutoSize = true;
            this.lblFechaDesde.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFechaDesde.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblFechaDesde.Location = new System.Drawing.Point(11, 118);
            this.lblFechaDesde.Name = "lblFechaDesde";
            this.lblFechaDesde.Size = new System.Drawing.Size(99, 29);
            this.lblFechaDesde.TabIndex = 7;
            this.lblFechaDesde.Text = "Desde:";
            // 
            // dtpFechaDesde
            // 
            this.dtpFechaDesde.Enabled = false;
            this.dtpFechaDesde.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFechaDesde.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaDesde.Location = new System.Drawing.Point(116, 112);
            this.dtpFechaDesde.Name = "dtpFechaDesde";
            this.dtpFechaDesde.Size = new System.Drawing.Size(130, 37);
            this.dtpFechaDesde.TabIndex = 8;
            // 
            // lblFechaHasta
            // 
            this.lblFechaHasta.AutoSize = true;
            this.lblFechaHasta.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFechaHasta.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblFechaHasta.Location = new System.Drawing.Point(256, 118);
            this.lblFechaHasta.Name = "lblFechaHasta";
            this.lblFechaHasta.Size = new System.Drawing.Size(92, 29);
            this.lblFechaHasta.TabIndex = 9;
            this.lblFechaHasta.Text = "Hasta:";
            // 
            // dtpFechaHasta
            // 
            this.dtpFechaHasta.Enabled = false;
            this.dtpFechaHasta.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtpFechaHasta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFechaHasta.Location = new System.Drawing.Point(354, 112);
            this.dtpFechaHasta.Name = "dtpFechaHasta";
            this.dtpFechaHasta.Size = new System.Drawing.Size(136, 37);
            this.dtpFechaHasta.TabIndex = 10;
            // 
            // btnBuscar
            // 
            this.btnBuscar.BackColor = System.Drawing.Color.SteelBlue;
            this.btnBuscar.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscar.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnBuscar.Location = new System.Drawing.Point(1133, 109);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(157, 46);
            this.btnBuscar.TabIndex = 11;
            this.btnBuscar.Text = "BUSCAR";
            this.btnBuscar.UseVisualStyleBackColor = false;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            // 
            // btnLimpiar
            // 
            this.btnLimpiar.BackColor = System.Drawing.Color.SteelBlue;
            this.btnLimpiar.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLimpiar.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnLimpiar.Location = new System.Drawing.Point(1307, 109);
            this.btnLimpiar.Name = "btnLimpiar";
            this.btnLimpiar.Size = new System.Drawing.Size(169, 46);
            this.btnLimpiar.TabIndex = 12;
            this.btnLimpiar.Text = "LIMPIAR";
            this.btnLimpiar.UseVisualStyleBackColor = false;
            this.btnLimpiar.Click += new System.EventHandler(this.btnLimpiar_Click);
            // 
            // gbRecibos
            // 
            this.gbRecibos.Controls.Add(this.dgvRecibos);
            this.gbRecibos.Font = new System.Drawing.Font("Verdana", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbRecibos.Location = new System.Drawing.Point(29, 237);
            this.gbRecibos.Name = "gbRecibos";
            this.gbRecibos.Size = new System.Drawing.Size(1589, 631);
            this.gbRecibos.TabIndex = 2;
            this.gbRecibos.TabStop = false;
            this.gbRecibos.Text = "Recibos de recepción";
            // 
            // dgvRecibos
            // 
            this.dgvRecibos.AllowUserToAddRows = false;
            this.dgvRecibos.AllowUserToDeleteRows = false;
            this.dgvRecibos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvRecibos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRecibos.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgvRecibos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRecibos.Location = new System.Drawing.Point(16, 40);
            this.dgvRecibos.MultiSelect = false;
            this.dgvRecibos.Name = "dgvRecibos";
            this.dgvRecibos.ReadOnly = true;
            this.dgvRecibos.RowHeadersWidth = 40;
            this.dgvRecibos.RowTemplate.Height = 33;
            this.dgvRecibos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvRecibos.Size = new System.Drawing.Size(1557, 571);
            this.dgvRecibos.TabIndex = 0;
            this.dgvRecibos.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRecibos_CellContentClick);
            this.dgvRecibos.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvRecibos_CellFormatting);
            // 
            // btnImprimirRemito
            // 
            this.btnImprimirRemito.BackColor = System.Drawing.Color.SteelBlue;
            this.btnImprimirRemito.Font = new System.Drawing.Font("Verdana", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnImprimirRemito.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.btnImprimirRemito.Location = new System.Drawing.Point(29, 882);
            this.btnImprimirRemito.Name = "btnImprimirRemito";
            this.btnImprimirRemito.Size = new System.Drawing.Size(332, 46);
            this.btnImprimirRemito.TabIndex = 12;
            this.btnImprimirRemito.Text = "IMPRIMIR REMITO";
            this.btnImprimirRemito.UseVisualStyleBackColor = false;
            this.btnImprimirRemito.Click += new System.EventHandler(this.btnImprimirRemito_Click);
            // 
            // VerRecibos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(1654, 940);
            this.Controls.Add(this.btnImprimirRemito);
            this.Controls.Add(this.gbRecibos);
            this.Controls.Add(this.gbFiltros);
            this.Controls.Add(this.lblInstruccion);
            this.ForeColor = System.Drawing.Color.LightSteelBlue;
            this.Name = "VerRecibos";
            this.Text = "VerRecibos";
            this.gbFiltros.ResumeLayout(false);
            this.gbFiltros.PerformLayout();
            this.gbRecibos.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecibos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInstruccion;
        private System.Windows.Forms.GroupBox gbFiltros;
        private System.Windows.Forms.Label lblNumeroOrden;
        private System.Windows.Forms.TextBox txtNumeroOrden;
        private System.Windows.Forms.Label lblProveedor;
        private System.Windows.Forms.ComboBox cboProveedor;
        private System.Windows.Forms.CheckBox chkUsarFecha;
        private System.Windows.Forms.Label lblEstado;
        private System.Windows.Forms.ComboBox cboEstado;
        private System.Windows.Forms.Label lblFechaDesde;
        private System.Windows.Forms.DateTimePicker dtpFechaDesde;
        private System.Windows.Forms.Label lblFechaHasta;
        private System.Windows.Forms.DateTimePicker dtpFechaHasta;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.Button btnLimpiar;
        private System.Windows.Forms.DataGridView dgvRecibos;
        private System.Windows.Forms.Button btnImprimirRemito;
        private System.Windows.Forms.GroupBox gbRecibos;
    }
}
