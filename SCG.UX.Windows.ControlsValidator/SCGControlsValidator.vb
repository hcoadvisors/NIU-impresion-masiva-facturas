Imports System.ComponentModel
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports System.Drawing

<ProvideProperty("Descripcion", GetType(Control))> _
<ProvideProperty("Requerido", GetType(Control))> _
<ProvideProperty("ExpresionRegular", GetType(Control))> _
<ProvideProperty("PuedeSerVacio", GetType(Control))> _
<ProvideProperty("ValorNulo", GetType(Control))> _
<ProvideProperty("Orden", GetType(Control))> _
<ToolboxBitmap(GetType(Bitmap))> _
<Localizable(True)> _
Public Class SCGControlsValidator
    Inherits Component
    Implements System.ComponentModel.IExtenderProvider

    ''' <summary>
    ''' Delegate para especificar un método a ejecutar cuando se vaya a mostrar un error en vez de usar el diálogo
    ''' predefinido
    ''' </summary>
    ''' <param name="control">Contenedor con los datos del Control</param>
    ''' <param name="tipoError">Tipo de error de validación</param>
    ''' <remarks></remarks>
    Public Delegate Sub MetodoMuestraError(ByVal control As ContenedorDelControl, ByVal tipoError As TipoError)

    ''' <summary>
    ''' Delegate para especificar definir el evento que se va a ejecutar cuando invoca la validación del control.
    ''' </summary>
    ''' <param name="control">Control que se está validando.</param>
    ''' <param name="mensajeError">En esta variable debe poner el mensaje de error que se debe mostrar
    ''' si no cumple con la validación.</param>
    ''' <param name="esValido">Aquí se pondrá el valor que indica si la validación del control fue correcta. El valor predeterminado es True</param>
    ''' <remarks></remarks>
    Public Delegate Sub MetodoOtrasValidaciones(ByVal control As ContenedorDelControl, ByRef mensajeError As String, ByRef esValido As Boolean)

    Private m_dictControlsList As Dictionary(Of Integer, ContenedorDelControl) = New Dictionary(Of Integer, ContenedorDelControl)()
    Private WithEvents m_parentForm As Form
    Private m_clrErrorColor As Color = Color.Beige
    Private m_strInvalidFormatText As String = "{0} : No está en el formato adecuado"
    Private m_strIsEmptyText As String = "{0}: No puede ser vacío."
    Private m_strNotDefinedText As String = "{0}: Está sin definir. Debe asignarle un valor."
    Private m_strErrorCaption As String = "Error en: {0}"
    Private m_ttipError As ToolTip
    Private m_errorSub As MetodoMuestraError = Nothing
    Private m_otrasValidSub As MetodoOtrasValidaciones = Nothing
    Private m_duracion As Integer = 5000

    Public Sub New()
    End Sub

    '''<summary>
    '''Specifies whether this object can provide its extender properties to the specified object.
    '''</summary>
    '''
    '''<returns>
    '''true if this object can provide extender properties to the specified object; otherwise, false.
    '''</returns>
    '''
    '''<param name="extendee">The <see cref="T:System.Object"></see> to receive the extender properties.</param>
    Public Function CanExtend(ByVal extendee As Object) As Boolean Implements IExtenderProvider.CanExtend
        If (TypeOf extendee Is Control AndAlso Not TypeOf extendee Is IExtenderProvider) Then Return True
        Return False
    End Function

#Region "Extended properties"
    <DefaultValue("False")> _
    <Category("SCG ControlValidator")> _
    <Description("Especifica si el control debe ser validado, o no.")> _
    Public Function GetRequerido(ByVal ctrl As Control) As Boolean
        Return ControlItem(ctrl).m_blnRequired
    End Function

    Public Sub SetRequerido(ByVal ctrl As Control, ByVal value As Boolean)
        ControlItem(ctrl).m_blnRequired = value
    End Sub

    <DefaultValue("False")> _
    <Category("SCG ControlValidator")> _
    <Description("Especifica si el texto del control puede ser la cadena vacía")> _
    Public Function GetPuedeSerVacio(ByVal ctrl As Control) As Boolean
        Return ControlItem(ctrl).m_blnCanBeEmpty
    End Function

    Public Sub SetPuedeSerVacio(ByVal ctrl As Control, ByVal value As Boolean)
        ControlItem(ctrl).m_blnCanBeEmpty = value
    End Sub

    <DefaultValue("")> _
    <Category("SCG ControlValidator")> _
    <Localizable(True)> _
    <Description("Descripción del campo que representa el control. Se usa en el momento de mostrar un error.")> _
    Public Function GetDescripcion(ByVal ctrl As Control) As String
        Return ControlItem(ctrl).m_strErrorText
    End Function

    Public Sub SetDescripcion(ByVal ctrl As Control, ByVal value As String)
        ControlItem(ctrl).m_strErrorText = value
    End Sub

    <DefaultValue(".*")> _
    <Category("SCG ControlValidator")> _
    <Description("Expresión regular que se va a usar para validar el texto del control")> _
    Public Function GetExpresionRegular(ByVal ctrl As Control) As String
        Return ControlItem(ctrl).m_strRegularExpressionText
    End Function

    Public Sub SetExpresionRegular(ByVal ctrl As Control, ByVal value As String)
        ControlItem(ctrl).m_strRegularExpressionText = value
    End Sub

    <DefaultValue("<Sin definir>")> _
    <Category("SCG ControlValidator")> _
    <Description("Cadena de texto que va a ser considerada como valor Nulo. Dejar en blanco para no considerar esta validación.")> _
    <Localizable(True)> _
    Public Function GetValorNulo(ByVal ctrl As Control) As String
        Return ControlItem(ctrl).m_strNullText
    End Function

    Public Sub SetValorNulo(ByVal ctrl As Control, ByVal value As String)
        ControlItem(ctrl).m_strNullText = value
    End Sub

    <DefaultValue("0")> _
    <Category("SCG ControlValidator")> _
    <Description("Orden en que se va a validar este control con respecto a los demás.")> _
    Public Function GetOrden(ByVal ctrl As Control) As Integer
        Return ControlItem(ctrl).m_intPriority
    End Function

    Public Sub SetOrden(ByVal ctrl As Control, ByVal value As Integer)
        ControlItem(ctrl).m_intPriority = value
    End Sub

#End Region

#Region "Control public properties"
    ''' <summary>
    ''' Formulario padre cuando se vaya a mostrar un error.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Browsable(True)> _
    <Category("SCG ControlValidator")> _
    <Description("Formulario padre cuando se vaya a mostrar un error.")> _
    Public Property FormularioPadre() As Form
        Get
            Return m_parentForm
        End Get
        Set(ByVal value As Form)
            m_parentForm = value
        End Set
    End Property
    ''' <summary>
    ''' Establece el tiempo en milisegundos que va a estar mostrado el mensaje de error.
    ''' </summary>
    ''' <value>Tiempo en milisegundos</value>
    ''' <returns>Tiempo en milisegundos</returns>
    ''' <remarks></remarks>
    <Browsable(True)> _
    <Category("SCG ControlValidator")> _
    <Description("Duración del mensaje de error.")> _
    Public Property Duracion() As Integer
        Get
            Return m_duracion
        End Get
        Set(ByVal value As Integer)
            m_duracion = value
        End Set
    End Property

    ''' <summary>
    ''' Color que va a tomar el control cuando ocurra un error de validación.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Browsable(True)> _
    <Category("SCG ControlValidator")> _
    <Description("Color que va a tomar el control cuando ocurra un error de validación.")> _
    Public Property ColorError() As Color
        Get
            Return m_clrErrorColor
        End Get
        Set(ByVal value As Color)
            m_clrErrorColor = value
        End Set
    End Property
    ''' <summary>
    ''' Error a mostrar cuando se produzca un error de validación porque no cumple con la expresión regular. {0} representa el texto que identifica el control (Descripcion).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Category("Mensajes")> _
    <DefaultValue("{0} : No está en el formato adecuado")> _
    <Localizable(True)> _
    <Description("Error a mostrar cuando se produzca un error de validación porque no cumple con la expresión regular. {0} representa el texto que identifica el control (Descripcion).")> _
    Public Property ErrorFormatoInvalido() As String
        Get
            Return m_strInvalidFormatText
        End Get
        Set(ByVal value As String)
            m_strInvalidFormatText = value
        End Set
    End Property

    ''' <summary>
    ''' Error a mostrar cuando se produzca un error de validación porque el texto está en blanco. {0} representa el texto que identifica el control (Descripcion).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Category("Mensajes")> _
    <DefaultValue("{0}: No puede ser vacío.")> _
    <Localizable(True)> _
    <Description("Error a mostrar cuando se produzca un error de validación porque el texto está en blanco. {0} representa el texto que identifica el control (Descripcion).")> _
    Public Property ErrorCadenaVacía() As String
        Get
            Return m_strIsEmptyText
        End Get
        Set(ByVal value As String)
            m_strIsEmptyText = value
        End Set
    End Property

    ''' <summary>
    ''' Error a mostrar cuando se produzca un error de validación porque el texto es el valor definido como nulo. {0} representa el texto que identifica el control (Descripcion).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Category("Mensajes")> _
    <DefaultValue("{0}: Está sin definir. Debe asignarle un valor.")> _
    <Localizable(True)> _
    <Description("Error a mostrar cuando se produzca un error de validación porque el texto es el valor definido como nulo. {0} representa el texto que identifica el control (Descripcion).")> _
    Public Property ErrorValorNulo() As String
        Get
            Return m_strNotDefinedText
        End Get
        Set(ByVal value As String)
            m_strNotDefinedText = value
        End Set
    End Property

    ''' <summary>
    ''' Título del mensaje cuando se muestra un error. {0} representa el texto que identifica el control (Descripcion).
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <Category("Mensajes")> _
    <DefaultValue("Error en: {0}")> _
    <Localizable(True)> _
    <Description("Título del mensaje cuando se muestra un error. {0} representa el texto que identifica el control (Descripcion).")> _
    Public Property ErrorTitulo() As String
        Get
            Return m_strErrorCaption
        End Get
        Set(ByVal value As String)
            m_strErrorCaption = value
        End Set
    End Property

    ''' <summary>
    ''' Metodo a ejecutar cuando se vaya a mostrar un mensaje de error.
    ''' </summary>
    ''' <value>Nombre del método</value>
    ''' <returns></returns>
    ''' <remarks>Si se especifica no se muestra el mensaje predeterminado</remarks>
    <Browsable(False)> _
    Public Property ErrorSub() As MetodoMuestraError
        Get
            Return m_errorSub
        End Get
        Set(ByVal value As MetodoMuestraError)
            m_errorSub = value
        End Set
    End Property

    ''' <summary>
    ''' Evento que se va a ejecutar antes de terminar la validación para que el usuario pueda
    ''' definir sus propias validaciones.
    ''' Este evento se va a llamar por cada control que se haya marcado como requerido.
    ''' </summary>
    ''' <remarks></remarks>
    Public Event OtrasValidaciones As MetodoOtrasValidaciones
#End Region

#Region "Public Methods"
    ''' <summary>
    ''' Causa que se chequeen los controles que se especificaron que se iban a validar para comprobar si cumplen
    ''' con las validaciones especificadas. Se irán validando según el orden especificado para cada control.
    ''' </summary>
    ''' <param name="muestraDialogos">
    ''' Especifica si se van a mostrar los diálogos de errores.
    ''' </param>
    ''' <returns>
    ''' True si todos los controles fueron validados correctamente. Falso en otro caso.
    ''' </returns>
    ''' <remarks></remarks>
    Public Overridable Function Valida(ByVal muestraDialogos As Boolean) As Boolean
        Dim ctrlItem As ContenedorDelControl
        Dim ctrlList As List(Of ContenedorDelControl) = New List(Of ContenedorDelControl)()

        For Each pair As KeyValuePair(Of Integer, ContenedorDelControl) In m_dictControlsList
            ctrlItem = pair.Value
            If (ctrlItem.m_blnRequired) Then ctrlList.Add(ctrlItem)
        Next
        ctrlList.Sort(AddressOf CompareControlItemsByPriority)
        Dim resultadoOtrasValidaciones As Boolean = True

        If (m_ttipError IsNot Nothing) Then m_ttipError.RemoveAll()
        For Each pair As ContenedorDelControl In ctrlList
            ctrlItem = pair
            Dim text As String = ctrlItem.m_ctrl.Text.Trim()

            'si no puede ser vacio y el text del control esta vacio: Error
            If (Not ctrlItem.m_blnCanBeEmpty AndAlso String.IsNullOrEmpty(text)) Then
                If (muestraDialogos) Then ShowError(TipoError.TextoEnBlanco, ctrlItem, String.Empty)
                Return False
            End If
            'si la expresion regular no es vacia entonces chequearla
            If (Not String.IsNullOrEmpty(ctrlItem.m_strRegularExpressionText)) Then
                Dim regex As Regex = New Regex(ctrlItem.m_strRegularExpressionText)
                If (Not regex.IsMatch(text)) Then
                    If (muestraDialogos) Then ShowError(TipoError.TextoEnBlanco, ctrlItem, String.Empty)
                    Return False
                End If
            End If
            'sin el ValorNulo no es vacio, chequear que el texto no sea igual al ValorNulo
            If (Not String.IsNullOrEmpty(ctrlItem.m_strNullText) AndAlso text = ctrlItem.m_strNullText) Then
                If (muestraDialogos) Then ShowError(TipoError.TextoValorNulo, ctrlItem, String.Empty)
                Return False
            End If
            '//////----//// mostrar error...
            Dim customError As String = String.Empty
            Dim esValido As Boolean = True
'            If (m_ttipError IsNot Nothing) Then m_ttipError.RemoveAll()
            RaiseEvent OtrasValidaciones(ctrlItem, customError, esValido)
            If (Not esValido) Then
                ShowError(TipoError.ErrorUsuario, ctrlItem, customError)
                Return False
            End If
        Next
        Return True
    End Function

    ''' <summary>
    ''' Limpia las notificaciones que se estén mostrando.
    ''' </summary>
    ''' <remarks></remarks>
    Public Overridable Sub Limpiar()
        If (m_ttipError IsNot Nothing) Then m_ttipError.RemoveAll()
    End Sub
#End Region

#Region "Private Methods"

    ''' <summary>
    ''' Muestra el mensaje de error.
    ''' </summary>
    ''' <param name="errType">Tipo de Error</param>
    ''' <param name="ctrlItem">Clase que contiene el control y sus parámetros.</param>
    ''' <param name="customErrorText">Mensaje de error que va a mostrar si es definido por el usuario.</param>
    ''' <remarks></remarks>
    Protected Overridable Sub ShowError(ByVal errType As TipoError, ByVal ctrlItem As ContenedorDelControl, ByVal customErrorText As String)
        If (m_errorSub IsNot Nothing) Then
            m_errorSub(ctrlItem, errType)
            Return
        End If
        Dim strError As String = ""
        '        ctrlItem.m_ctrl.BackColor = m_clrErrorColor
        Select Case errType
            Case TipoError.ErrorExpresionRegular
                strError = String.Format(m_strInvalidFormatText, ctrlItem.m_strErrorText)
            Case TipoError.TextoEnBlanco
                strError = String.Format(m_strIsEmptyText, ctrlItem.m_strErrorText)
            Case TipoError.TextoValorNulo
                strError = String.Format(m_strNotDefinedText, ctrlItem.m_strErrorText)
            Case TipoError.ErrorUsuario
                strError = customErrorText
        End Select
        m_ttipError = New ToolTip()
        m_ttipError.InitialDelay = 0
        '        m_ttipError.AutoPopDelay = 3000
        m_ttipError.UseFading = True
        m_ttipError.UseAnimation = True
        m_ttipError.IsBalloon = True
        m_ttipError.BackColor = m_clrErrorColor
        m_ttipError.ToolTipIcon = ToolTipIcon.Error
        m_ttipError.SetToolTip(ctrlItem.m_ctrl, strError)
        m_ttipError.Show(strError, ctrlItem.m_ctrl, m_duracion)
        ctrlItem.m_ctrl.Focus()

    End Sub

    ''' <summary>
    ''' Adiciona un control en el dicccionario. 
    ''' </summary>
    ''' <param name="ctrl">Control a añadir.</param>
    ''' <returns>Referencia a la entrada en el diccionario.</returns>
    ''' <remarks></remarks>
    Protected Overridable Function ControlItem(ByVal ctrl As Control) As ContenedorDelControl
        If (Not m_dictControlsList.ContainsKey(ctrl.GetHashCode())) Then
            m_dictControlsList.Add(ctrl.GetHashCode(), New ContenedorDelControl())
            m_dictControlsList(ctrl.GetHashCode()).m_ctrl = ctrl
            m_dictControlsList(ctrl.GetHashCode()).m_clrBackColor = ctrl.BackColor
        End If
        Return m_dictControlsList(ctrl.GetHashCode())
    End Function

    ''' <summary>
    ''' Metodo que se usa para comparar segun el priority(Orden)
    ''' </summary>
    ''' <param name="c1"></param>
    ''' <param name="c2"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CompareControlItemsByPriority(ByVal c1 As ContenedorDelControl, ByVal c2 As ContenedorDelControl) As Integer
        Return Math.Sign(c1.m_intPriority - c2.m_intPriority)
    End Function

#End Region
End Class