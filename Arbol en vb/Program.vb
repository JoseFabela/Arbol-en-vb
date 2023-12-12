Imports Newtonsoft.Json
Imports System.IO

Public Class Tree
    Public Property Root As TreeNode

    Public Sub New(root As TreeNode)
        Me.Root = root
    End Sub

    Public Sub PrintTree(node As TreeNode, Optional indent As String = "")
        Console.WriteLine(indent & "└─ " & node.Name)

        For i As Integer = 0 To node.Children.Count - 1
            PrintTree(node.Children(i), indent & "   ")
        Next
    End Sub

    Public Function FindNode(nodeName As String, Optional node As TreeNode = Nothing) As TreeNode
        If node Is Nothing Then
            node = Root
        End If

        If node.Name = nodeName Then
            Return node
        End If

        For Each child In node.Children
            Dim foundNode = FindNode(nodeName, child)
            If foundNode IsNot Nothing Then
                Return foundNode
            End If
        Next

        Return Nothing
    End Function

    Public Sub AddNode(parentNodeName As String, newNodeName As String)
        Dim parentNode = FindNode(parentNodeName)
        If parentNode IsNot Nothing Then
            parentNode.Children.Add(New TreeNode(newNodeName))
        Else
            Console.WriteLine($"No se encontró el nodo padre '{parentNodeName}'.")
        End If
    End Sub

    Public Sub EditNode(nodeName As String, newName As String)
        Dim nodeToEdit = FindNode(nodeName)
        If nodeToEdit IsNot Nothing Then
            nodeToEdit.Name = newName
        Else
            Console.WriteLine($"No se encontró el nodo '{nodeName}'.")
        End If
    End Sub

    Public Sub DeleteNode(nodeName As String)
        Dim nodeToDelete = FindNode(nodeName)
        If nodeToDelete IsNot Nothing Then
            If nodeToDelete Is Root Then
                ' No se puede eliminar el nodo raíz
                Console.WriteLine("No se puede eliminar el nodo raíz.")
            Else
                Dim parent = FindParentNode(nodeName)
                If parent IsNot Nothing Then
                    If nodeToDelete.Children.Count > 0 Then
                        ' Convertir el primer hijo en el nuevo padre
                        Dim firstChild = nodeToDelete.Children(0)
                        firstChild.Children.AddRange(nodeToDelete.Children.Skip(1))
                        parent.Children.Insert(parent.Children.IndexOf(nodeToDelete), firstChild)
                    End If
                    parent.Children.Remove(nodeToDelete)
                    Console.WriteLine($"El nodo '{nodeName}' se eliminó, y el primer hijo se convirtió en el nuevo padre sin cambiar la posición de la rama.")
                End If
            End If
        Else
            Console.WriteLine($"No se encontró el nodo '{nodeName}'.")
        End If
    End Sub

    Private Function FindParentNode(nodeName As String, Optional node As TreeNode = Nothing) As TreeNode
        If node Is Nothing Then
            node = Root
        End If

        For Each child In node.Children
            If child.Name = nodeName Then
                Return node
            End If

            Dim parent = FindParentNode(nodeName, child)
            If parent IsNot Nothing Then
                Return parent
            End If
        Next

        Return Nothing
    End Function

    ' Guardar la estructura del árbol en un archivo JSON
    Public Sub SaveTreeToFile(filePath As String)
        Dim json = JsonConvert.SerializeObject(Me)
        File.WriteAllText(filePath, json)
        Console.WriteLine("Estructura del árbol guardada en el archivo.")
    End Sub

    ' Cargar la estructura del árbol desde un archivo JSON
    Public Shared Function LoadTreeFromFile(filePath As String) As Tree
        If File.Exists(filePath) Then
            Dim json = File.ReadAllText(filePath)
            Dim tree = JsonConvert.DeserializeObject(Of Tree)(json)
            Console.WriteLine("Estructura del árbol cargada desde el archivo.")
            Return tree
        Else
            Console.WriteLine("El archivo no existe. Se creará un nuevo árbol.")
            Return Nothing
        End If
    End Function
End Class
Public Class TreeNode
    Public Property Name As String
    Public ReadOnly Property Children As New List(Of TreeNode)

    Public Sub New(name As String)
        Me.Name = name
    End Sub
End Class

