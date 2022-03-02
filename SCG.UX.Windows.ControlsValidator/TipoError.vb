''' <summary>
''' Tipos de errror de validaci�n.
''' </summary>
''' <remarks></remarks>
Public Enum TipoError
    ''' <summary>
    ''' El error ocurri� porque el texto del control se encontraba en blanco.
    ''' </summary>
    ''' <remarks></remarks>
    TextoEnBlanco
    '''<summary>
    ''' El error ocurri� porque el texto del control no cumple con la expresi�n regular especificada.
    ''' </summary>
    ''' <remarks></remarks>
    ErrorExpresionRegular
    '''<summary>
    ''' El error ocurri� porque el texto del control era igual a la cadena especificada como ValorNulo.
    ''' </summary>
    ''' <remarks></remarks>
    TextoValorNulo
    ''' <summary>
    ''' El error es por una validaci�n definida por el usuario.
    ''' </summary>
    ''' <remarks></remarks>
    ErrorUsuario
End Enum