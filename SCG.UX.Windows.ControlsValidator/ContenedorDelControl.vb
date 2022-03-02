Imports System.Windows.Forms
Imports System.Drawing

''' <summary>
''' Objeto que representa el control y las propiedades que se van a usar cuando
''' se vaya a validar.
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class ContenedorDelControl
    Friend m_strRegularExpressionText As String = ".*"
    Friend m_strErrorText As String = String.Empty
    Friend m_blnRequired As Boolean
    Friend m_blnCanBeEmpty As Boolean
    Friend m_strNullText As String = "<Sin definir>"
    Friend WithEvents m_ctrl As Control
    Friend m_clrBackColor As Color
    Friend m_intPriority As Integer = 0

    Friend Sub New()
    End Sub

    ''' <summary>
    ''' Descripción del campo que representa el control.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Descripcion() As String
        Get
            Return m_strErrorText
        End Get
        Set(ByVal value As String)
            m_strErrorText = value
        End Set
    End Property

    ''' <summary>
    ''' Especifica si el control debe ser validado, o no.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Requerido() As Boolean
        Get
            Return m_blnRequired
        End Get
        Set(ByVal value As Boolean)
            m_blnRequired = value
        End Set
    End Property

    ''' <summary>
    ''' Expresión regular que se va a usar para validar el texto del control.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ExpresionRegular() As String
        Get
            Return m_strRegularExpressionText
        End Get
        Set(ByVal value As String)
            m_strRegularExpressionText = value
        End Set
    End Property

    ''' <summary>
    ''' Especifica si el texto del control puede ser la cadena vacía
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property PuedeSerVacio() As Boolean
        Get
            Return m_blnCanBeEmpty
        End Get
        Set(ByVal value As Boolean)
            m_blnCanBeEmpty = value
        End Set
    End Property

    ''' <summary>
    ''' Cadena de texto que va a ser considerada como valor Nulo.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property ValorNulo() As String
        Get
            Return m_strNullText
        End Get
        Set(ByVal value As String)
            m_strNullText = value
        End Set
    End Property

    ''' <summary>
    ''' Control asociado
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Ctrl() As Control
        Get
            Return m_ctrl
        End Get
        Set (ByVal value As Control)
            m_ctrl = value
        End Set
    End Property

    ''' <summary>
    ''' Orden en que se va a validar este control con respecto a los demás.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Orden() As Integer
        Get
            Return m_intPriority
        End Get
        Set(ByVal value As Integer)
            m_intPriority = value
        End Set
    End Property
End Class
