Imports System.IO
Imports System.Net
Public Class Form1

#Region "Score Display and Sorting"

    Public ScoresMapATimed As New List(Of Integer)
    Public ScoresMapAUntimed As New List(Of Integer)
    Public ScoresMapBTimed As New List(Of Integer)
    Public ScoresMapBuntimed As New List(Of Integer)
    Private Function Bubblesort(MapScoresArray)

        Dim Sorted As Boolean
        Dim TempNum As Integer

        Do Until Sorted = True
            'Brings next biggest value to the top 
            For n As Integer = 0 To MapScoresArray.length - 2

                If MapScoresArray(n) > MapScoresArray(n + 1) Then
                    TempNum = MapScoresArray(n)
                    MapScoresArray(n) = MapScoresArray(n + 1)
                    MapScoresArray(n + 1) = TempNum
                End If

            Next

            'Checks if sorted 
            For n As Integer = 0 To MapScoresArray.length - 2

                If MapScoresArray(n) > MapScoresArray(n + 1) Then
                    Continue Do
                End If

            Next

            Sorted = True

        Loop

        Return MapScoresArray

    End Function

    'Clears all lists so that Scores that are being input dont get repeated
    Public Sub ClearStatLists()
        LstBoxMapAUntimed.Items.Clear()
        LstBoxMapBUntimed.Items.Clear()
        LstBoxMapATimed.Items.Clear()
        LstBoxMapBTimed.Items.Clear()
    End Sub

    Private Sub BtnDisplayLocalData_Click(sender As Object, e As EventArgs) Handles BtnDisplayLocalData.Click
        DisplayLocalScores()
    End Sub

    Private Sub DisplayLocalData_Click(sender As Object, e As EventArgs) Handles DisplayLocalData.Click
        DisplayLocalScores()
    End Sub

    Private Sub DisplayLocalScores()

        ClearStatLists()

        'Sorts and displays local variables for local scores in each category

        Dim MapScoresArray() As Integer = Bubblesort(ScoresMapATimed.ToArray)

        For Each N As Integer In MapScoresArray.Reverse
            LstBoxMapATimed.Items.Add(N)
        Next

        MapScoresArray = Bubblesort(ScoresMapAUntimed.ToArray)

        For Each N As Integer In MapScoresArray.Reverse
            LstBoxMapAUntimed.Items.Add(N)
        Next

        MapScoresArray = Bubblesort(ScoresMapBTimed.ToArray)

        For Each N As Integer In MapScoresArray.Reverse
            LstBoxMapBTimed.Items.Add(N)
        Next

        MapScoresArray = Bubblesort(ScoresMapBuntimed.ToArray)

        For Each N As Integer In MapScoresArray.Reverse
            LstBoxMapBUntimed.Items.Add(N)
        Next

    End Sub
    Private Sub BtnDisplayGlobalData_Click(sender As Object, e As EventArgs) Handles BtnDisplayGlobalData.Click
        DisplayGlobalScores()
    End Sub
    Private Sub DisplayGlobalData_Click(sender As Object, e As EventArgs) Handles DisplayGlobalData.Click
        DisplayGlobalScores()
    End Sub

    Private Sub DisplayGlobalScores()

        ClearStatLists()

        'Inports and sorts Global Timed Scorse for Map 1 and displayes them into correct list
        Dim TempGlobalScoreArray() As Integer = LoadScores(Application.StartupPath & "GlobalTimedScorseMap1.txt")

        Bubblesort(TempGlobalScoreArray)

        For Each N As Integer In TempGlobalScoreArray.Reverse
            LstBoxMapATimed.Items.Add(N)
        Next

        'Inports and sorts Global Untimed Scorse for Map 1 and displayes them into correct list
        TempGlobalScoreArray = LoadScores(Application.StartupPath & "GlobalUntimedScorseMap1.txt")

        Bubblesort(TempGlobalScoreArray)

        For Each N As Integer In TempGlobalScoreArray.Reverse
            LstBoxMapAUntimed.Items.Add(N)
        Next

        'Inports and sorts Global Timed Scorse for Map 2 and displayes them into correct list
        TempGlobalScoreArray = LoadScores(Application.StartupPath & "GlobalTimedScorseMap2.txt")

        Bubblesort(TempGlobalScoreArray)

        For Each N As Integer In TempGlobalScoreArray.Reverse
            LstBoxMapBTimed.Items.Add(N)
        Next

        'Inports and sorts Global Untimed Scorse for Map 2 and displayes them into correct list
        TempGlobalScoreArray = LoadScores(Application.StartupPath & "GlobalUntimedScorseMap2.txt")

        Bubblesort(TempGlobalScoreArray)

        For Each N As Integer In TempGlobalScoreArray.Reverse
            LstBoxMapBUntimed.Items.Add(N)
        Next

    End Sub

    Private Function LoadScores(Filepath)

        Dim TempIntArray(999999) As Integer
        Dim Count As Integer = 0

        'Gets the Scores from the file location and saves into a temporary array 
        If File.Exists(Filepath) Then
            Dim lines() As String = File.ReadAllLines(Filepath)

            For Each line In lines

                TempIntArray(Count) = CInt(line)

                Count += 1

                If Count = 200 Then
                    Exit For
                End If

            Next

        Else
            'If the file location is not found message gets output
            MsgBox("File not found.")
            Return Nothing
        End If

        Dim TotalNumbers As Integer = 0

        'Finds out how maany values are non 0 
        For Each num In TempIntArray
            If num = 0 Then
                Exit For
            End If
            TotalNumbers += 1

        Next

        Dim IntArray(TotalNumbers - 1) As Integer

        Count = 0

        'Condenses list to all real values
        Do Until Count = TotalNumbers

            IntArray(Count) = TempIntArray(Count)

            Count += 1
        Loop

        Return IntArray

    End Function

#End Region

#Region "Visability Commands"

    Public PlayerSelelectedColour As Brush = Brushes.Yellow
    Private Sub BtnStart_Click(sender As Object, e As EventArgs) Handles BtnStart.Click
        PnlOptions.Visible = False
        PnlStats.Visible = False

        Form2.Visible = True

    End Sub

    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        Close()
    End Sub

    Private Sub BtnXOptions_Click(sender As Object, e As EventArgs) Handles BtnXOptions.Click
        PnlOptions.Visible = False
    End Sub

    Private Sub BtnOptions_Click(sender As Object, e As EventArgs) Handles BtnOptions.Click
        PnlOptions.Visible = True
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Call CenterToScreen()
        Me.WindowState = FormWindowState.Maximized

        PnlOptions.Visible = False
        PnlStats.Visible = False

    End Sub

    Private Sub BtnStats_Click(sender As Object, e As EventArgs) Handles BtnStats.Click
        PnlStats.Visible = True
    End Sub

    Private Sub BtnXStats_Click(sender As Object, e As EventArgs) Handles BtnXStats.Click
        PnlStats.Visible = False
    End Sub

#End Region

#Region "Player Custom Colour Selector"
    Private Sub UISelectColourDisplay()

        PbxSelectedCyan.Visible = False
        PbxSelectedFuchsia.Visible = False
        PbxSelectedGray.Visible = False
        PbxSelectedGrayBlue.Visible = False
        PbxSelectedLime.Visible = False
        PbxSelectedOrange.Visible = False
        PbxSelectedRed.Visible = False
        PbxSelectedYellow.Visible = False

    End Sub
    Private Sub BtnGrayChrColour_Click(sender As Object, e As EventArgs) Handles BtnGrayChrColour.Click

        UISelectColourDisplay()
        PbxSelectedGray.Visible = True

        PlayerSelelectedColour = Brushes.Gray

    End Sub

    Private Sub BtnRedChrColour_Click(sender As Object, e As EventArgs) Handles BtnRedChrColour.Click

        UISelectColourDisplay()
        PbxSelectedRed.Visible = True

        PlayerSelelectedColour = Brushes.Maroon

    End Sub

    Private Sub BtnOrangeChrColour_Click(sender As Object, e As EventArgs) Handles BtnOrangeChrColour.Click

        UISelectColourDisplay()
        PbxSelectedOrange.Visible = True

        PlayerSelelectedColour = Brushes.Orange

    End Sub

    Private Sub BtnYellowChrColour_Click(sender As Object, e As EventArgs) Handles BtnYellowChrColour.Click

        UISelectColourDisplay()
        PbxSelectedYellow.Visible = True

        PlayerSelelectedColour = Brushes.Yellow

    End Sub

    Private Sub BtnLimeChrColour_Click(sender As Object, e As EventArgs) Handles BtnLimeChrColour.Click

        UISelectColourDisplay()
        PbxSelectedLime.Visible = True

        PlayerSelelectedColour = Brushes.Lime

    End Sub

    Private Sub BtnCyanChrColour_Click(sender As Object, e As EventArgs) Handles BtnCyanChrColour.Click

        UISelectColourDisplay()
        PbxSelectedCyan.Visible = True

        PlayerSelelectedColour = Brushes.Cyan

    End Sub

    Private Sub BtnBlueChrColour_Click(sender As Object, e As EventArgs) Handles BtnBlueChrColour.Click

        UISelectColourDisplay()
        PbxSelectedGrayBlue.Visible = True

        PlayerSelelectedColour = Brushes.Blue

    End Sub

    Private Sub BtnFuchsiaChrColour_Click(sender As Object, e As EventArgs) Handles BtnFuchsiaChrColour.Click

        UISelectColourDisplay()
        PbxSelectedFuchsia.Visible = True

        PlayerSelelectedColour = Brushes.Fuchsia

    End Sub

#End Region

End Class
