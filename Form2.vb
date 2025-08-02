
#Region "Imports"

Imports System.ComponentModel
Imports System.Diagnostics.Tracing
Imports System.Drawing
Imports System.Drawing.Text
Imports System.IO
Imports Microsoft.VisualBasic.Devices
Imports Windows.Win32.UI
Imports System.Collections.Generic
Imports System.Formats.Asn1.AsnWriter
Imports System.Xml
Imports System.Runtime.InteropServices.JavaScript.JSType
Imports System.Net.Security
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar
Imports System.Runtime.CompilerServices
Imports System.Runtime.Intrinsics.Arm
Imports System.Net.NetworkInformation
Imports System.Reflection.Metadata.Ecma335
Imports System.Security.Cryptography.X509Certificates
Imports System.Net

#End Region
Public Class Form2

#Region "Visability and screen adjustments"
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Form1.Visible = False

        Call CenterToScreen()
        Me.WindowState = FormWindowState.Maximized


        lblInfo.Visible = True
        PnlGameOptions.Visible = False
        PnlGameStats.Visible = False
    End Sub

#End Region

#Region "All Starting Dimentions for Game Declaring"


    'Game Running 
    Private BoolIsRunning As Boolean = True
    Private Gameover As Boolean
    Private GameReset As Boolean = False

    Private GameoverStage As Integer
    'Stage 1 = Stop the main game running which halts display and pathfinding prossessing 
    'Stage 2 = displays all player stats of that game 
    'Stage 3 = Halts program for 5 seconds and then exits to main menu 

    'Modes
    Private PaintTrails As Boolean = False
    Private Speedx10 As Boolean = False
    Private Disco As Boolean = False
    Private DiscoCount As Integer = 1
    Private LightsOut As Boolean = False
    Private BlackAndWhite As Boolean = False
    Private InvertedControles As Boolean = False

    'Player being declared as Pacman
    Private Pacman As Pacman

    'Enemies as Ghost and other Ghost variables
    Private RedGhost As Redghost
    Private PinkGhost As PinkGhost
    Private BlueGhost As BlueGhost
    Private OrangeGhost As OrangeGhost

    Private Modecount As Integer 'Keeps track of time until mods change (also used to save time when in frightened mode)
    Private ScatterTimeCountTotal As Integer 'What the modecount will count up to change from scatter code to chase code 
    Private ChaseTimeCountTotal As Integer 'What the mode count will count up to change chase code to scatter code
    Private ModeScatter As Boolean = True 'This boolean allows to switch to scatter mode code from chase mode and vice versa

    'Map Size
    Private IntMapWidth As Integer
    Private IntMaphight As Integer

    '2D Array And Map display
    Private Maps As New List(Of Integer(,))()
    Private DiscoTrailMaps As New List(Of Integer(,))()
    Public MapNo As Integer = 0

    'Hard coded Maps
    '        0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27
    Private mapA(,) As Integer = {
            {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},  '0
            {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '1
            {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '2
            {2, 3, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 3, 2},  '3
            {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '4
            {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '5
            {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '6
            {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '7
            {2, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 2},  '8
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '9
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '10
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '11
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 5, 5, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '12
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 4, 4, 4, 4, 4, 4, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '13
            {-1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 4, 4, 4, 4, 4, 4, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1},'14
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 4, 4, 4, 4, 4, 4, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '15
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '16
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '17
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '18
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '19
            {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '20
            {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '21
            {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '22
            {2, 3, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 3, 2},  '23
            {2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2},  '24
            {2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2},  '25
            {2, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 2},  '26
            {2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2},  '27
            {2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2},  '28
            {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '29
            {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2}   '30
        }
    'Hard coded Maps
    '        0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27
    Private DiscoTrailsMapA(,) As Integer = {
            {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},  '0
            {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '1
            {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '2
            {2, 3, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 3, 2},  '3
            {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '4
            {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '5
            {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '6
            {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '7
            {2, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 2},  '8
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '9
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '10
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '11
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '12
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 1, 1, 1, 1, 1, 1, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '13
            {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},'14
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 1, 1, 1, 1, 1, 1, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '15
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '16
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '17
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '18
            {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '19
            {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '20
            {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '21
            {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '22
            {2, 3, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 3, 2},  '23
            {2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2},  '24
            {2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2},  '25
            {2, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 2},  '26
            {2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2},  '27
            {2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2},  '28
            {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '29
            {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2}   '30
        }
    Private mapB(,) As Integer = {
                {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1, 2, 2, -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},  '0
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '1
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '2
                {2, 3, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 3, 2},  '3
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '4
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '5
                {2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2},  '6
                {2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2},  '7
                {2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2},  '8
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '9
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '10
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '11
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 5, 5, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '12
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 4, 4, 4, 4, 4, 4, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '13
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 4, 4, 4, 4, 4, 4, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '14
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 4, 4, 4, 4, 4, 4, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '15
                {-1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, -1},  '16
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '17
                {2, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 2},  '18
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '19
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '20
                {2, 3, 2, 2, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 2, 2, 3, 2},  '21
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '22
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '23
                {2, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 2},  '24
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '25
                {2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2},  '26
                {2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2},  '27
                {2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2},  '28
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '29
                {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1, 2, 2, -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2}   '30
            }

    Private DiscoTrailsMapB(,) As Integer = {
                {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},  '0
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '1
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '2
                {2, 3, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 3, 2},  '3
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '4
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '5
                {2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2},  '6
                {2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2},  '7
                {2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2},  '8
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '9
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '10
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '11
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '12
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 1, 1, 1, 1, 1, 1, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '13
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 1, 1, 1, 1, 1, 1, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '14
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 1, 1, 1, 1, 1, 1, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '15
                {1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},  '16
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '17
                {2, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 2},  '18
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '19
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '20
                {2, 3, 2, 2, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 2, 2, 3, 2},  '21
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '22
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '23
                {2, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 2},  '24
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '25
                {2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2},  '26
                {2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2},  '27
                {2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2},  '28
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '29
                {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2}   '30
            }


    Private IntResWidth As Integer
    Private IntResHeight As Integer
    Private IntTileSize As Integer = 40
    Private TilesNotEmpty As Integer = 0
    Private IsGhostsFrightened As Boolean = False
    Private FrightenedGhostcount As Integer = 0

    'Graphics Variables 
    Private GFX As Graphics
    Private BackBufferGFX As Graphics
    Private BackBuffer As Bitmap
    Private r As Rectangle

    'Fps Counter & other
    Private InttSeconds As Integer = TimeOfDay.Second
    Private InttTicks As Integer = 0
    Private IntMaxTicks As Integer = 0
    Private IntTime As Integer = 0
    Private IsTimedlevel As Boolean = True

    'Ghost Counts and other 
    Private GhostActivationCount As Integer = 0
    Private IntGhostUnFrightenCountdown As Integer = 0
    Private GhostFrightenedTime As Integer = 8

    '' Buttons for this Form ''

    'Button that opens the exit pannel
    Private Sub LblClose_Click(sender As Object, e As EventArgs) Handles LblClose.Click
        BoolIsRunning = False
        PnlExit.Visible = True
    End Sub
    Private Sub BtnClose_Click(sender As Object, e As EventArgs) Handles BtnClose.Click
        BoolIsRunning = False
        PnlExit.Visible = True
    End Sub

    'Closes present form 
    Private Sub LblYes_Click(sender As Object, e As EventArgs) Handles LblYes.Click
        Form1.Visible = True
        Close()
    End Sub
    Private Sub BtnYes_Click(sender As Object, e As EventArgs) Handles BtnYes.Click
        Form1.Visible = True
        Close()
    End Sub

    'Closes exit pannel and goes back to game 
    Private Sub LblNo_Click(sender As Object, e As EventArgs) Handles LblNo.Click
        PnlExit.Visible = False
        BoolIsRunning = True
    End Sub

    Private Sub BtnNo_Click(sender As Object, e As EventArgs) Handles BtnNo.Click
        PnlExit.Visible = False
        BoolIsRunning = True
    End Sub

    'Initialises Game 
    Public Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.Show()
        Me.Focus()

        Maps.Add(mapA)
        Maps.Add(mapB)

        DiscoTrailMaps.Add(DiscoTrailsMapA)
        DiscoTrailMaps.Add(DiscoTrailsMapB)

        PnlExit.Visible = False
        PnlGameOptions.Visible = True

        Ghost.WeightedGhostMaps.Add(Ghost.WeightedGhostMapA)
        Ghost.WeightedGhostMaps.Add(Ghost.WeightedGhostMapB)

        Ghost.GhostMaps.Add(Ghost.GhostMapA)
        Ghost.GhostMaps.Add(Ghost.GhostMapB)

        MapNo = 0

    End Sub

    Private Sub BtnStart_Click(sender As Object, e As EventArgs) Handles BtnStart.Click
        StartGame()
    End Sub
    Private Sub LblStart_Click(sender As Object, e As EventArgs) Handles LblStart.Click
        StartGame()
    End Sub
    Private Sub StartGame()

        'Reset Fps Counter & other
        InttSeconds = TimeOfDay.Second
        InttTicks = 0
        IntMaxTicks = 0
        IntTime = 0
        GhostActivationCount = 0

        IsGhostsFrightened = False 'Boolean for activating the frightened code for ghosts
        FrightenedGhostcount = 0 'Keeps track of how long ghosts have been in the frightened state

        TilesNotEmpty = 0

        'Visability changes to clear screen when player starts game 
        PnlGameOptions.Visible = False
        lblInfo.Visible = False

        'Temporary setup: the map size will be declared in another form when UI is completed for now its declared here
        IntMapWidth = MapDisplay1.Size.Width
        IntMaphight = MapDisplay1.Size.Height

        IntResHeight = IntMaphight
        IntResWidth = IntMapWidth

        'Enable selected modes 
        If lblPaintTrailsONOFF.Text = "On" Then
            PaintTrails = True
        End If

        If lblSpeedX10ONOFF.Text = "On" Then
            Speedx10 = True
        End If

        If lblDiscoONOFF.Text = "On" Then
            Disco = True
        End If

        If lblLightsOutONOFF.Text = "On" Then
            LightsOut = True
        End If

        If lblBlackWhiteONOFF.Text = "On" Then
            BlackAndWhite = True
        End If

        If lblInvertedONOFF.Text = "On" Then
            InvertedControles = True
        End If

        'Declareing Pacman
        Pacman = New Pacman(13, 23)

        'Declareing ghosts 
        RedGhost = New Redghost(13, 11) '9, 11
        PinkGhost = New PinkGhost(12, 14) '13, 11
        OrangeGhost = New OrangeGhost(13, 14) '13, 11
        BlueGhost = New BlueGhost(14, 14) '13, 11

        'Blinky is only Ghost active straight away 
        RedGhost.IsActive = True   'Active right away 
        RedGhost.PreviousNodeX = 15
        RedGhost.PreviousNodeY = 11
        RedGhost.PreviousTileX = 14
        RedGhost.PreviousTileY = 11

        'Pinky is active when Blinky in his corner 
        PinkGhost.IsActive = False
        OrangeGhost.IsActive = False 'Active after first cycle
        BlueGhost.IsActive = False 'Active when first chase cycle comes 

        'Declaring squears for scatter mode 
        'TrackingSQIX/IY(0) represents the best scatter square where the Ghost should go 
        'TrackingSQIX/IY(1) represents the next best scatter square where the Ghost should go 
        'TrackingSQIX/IY(2)(3) Are declared so that the ghosts go the correct direction when in their scatter corners (Blinky = clockwise) (Pinky = anticlockwise)

        'Declare red ghots scatter positions  
        RedGhost.TrackingSQIX(0) = 26
        RedGhost.TrackingSQIY(0) = 1
        RedGhost.TrackingSQIX(1) = 26
        RedGhost.TrackingSQIY(1) = 5
        RedGhost.TrackingSQIX(2) = 21
        RedGhost.TrackingSQIY(2) = 5
        RedGhost.TrackingSQIX(3) = 15
        RedGhost.TrackingSQIY(3) = 5


        RedGhost.Scatter = True

        'Pink Ghost
        PinkGhost.TrackingSQIX(0) = 1
        PinkGhost.TrackingSQIY(0) = 1
        PinkGhost.TrackingSQIX(1) = 6
        PinkGhost.TrackingSQIY(1) = 1
        PinkGhost.TrackingSQIX(2) = 1
        PinkGhost.TrackingSQIY(2) = 5
        PinkGhost.TrackingSQIX(3) = 12
        PinkGhost.TrackingSQIY(3) = 5

        PinkGhost.Scatter = True

        'Blue Ghost 

        BlueGhost.TrackingSQIX(0) = 26
        BlueGhost.TrackingSQIY(0) = 29
        BlueGhost.TrackingSQIX(1) = 26
        BlueGhost.TrackingSQIY(1) = 27
        BlueGhost.TrackingSQIX(2) = 24
        BlueGhost.TrackingSQIY(2) = 29

        BlueGhost.Scatter = True

        'Orange Ghost 

        OrangeGhost.TrackingSQIX(0) = 1
        OrangeGhost.TrackingSQIY(0) = 29
        OrangeGhost.TrackingSQIX(1) = 1
        OrangeGhost.TrackingSQIY(1) = 27
        OrangeGhost.TrackingSQIX(2) = 3
        OrangeGhost.TrackingSQIY(2) = 29

        OrangeGhost.Scatter = True

        'Graphics objects
        GFX = MapDisplay1.CreateGraphics
        BackBuffer = New Bitmap(IntResWidth, IntResHeight)

        'Ensables timers
        Control.Start()
        GhostTimer.Start()
        Timer.Start()

    End Sub

#End Region

#Region "Graphical Interface"
    Private Sub DrawGraphics()

        Dim StartRenderX As Integer
        Dim EndRenderX As Integer
        Dim StartRenderY As Integer
        Dim EndRenderY As Integer

        If LightsOut = True Then
            StartRenderX = Pacman.IX - 5
            EndRenderX = Pacman.IX + 5
            StartRenderY = Pacman.IY - 5
            EndRenderY = Pacman.IY + 5
            If StartRenderX < 0 Then
                StartRenderX = 0
            End If
            If StartRenderY < 0 Then
                StartRenderY = 0
            End If
            If EndRenderX > 27 Then
                EndRenderX = 27
            End If
            If EndRenderY > 30 Then
                EndRenderY = 30
            End If
        Else
            StartRenderX = 0
            EndRenderX = 27
            StartRenderY = 0
            EndRenderY = 30
        End If

        'Copying BackBuffer to graphics object 
        Using GFX As Graphics = Graphics.FromImage(BackBuffer)

            If DiscoCount = 1 Then
                Pacman.DrawOverPreviousLocation(GFX, IntTileSize, Maps(MapNo), PaintTrails, Pacman.NoPelletsLeft)

                If (StartRenderX <= RedGhost.IX <= EndRenderX) And (StartRenderY <= RedGhost.IY <= EndRenderY) Then
                    RedGhost.DrawOverPreviousLocation(GFX, IntTileSize, Maps(MapNo), PaintTrails, Pacman.NoPelletsLeft)
                End If
                If (StartRenderX <= PinkGhost.IX <= EndRenderX) And (StartRenderY <= PinkGhost.IY <= EndRenderY) Then
                    PinkGhost.DrawOverPreviousLocation(GFX, IntTileSize, Maps(MapNo), PaintTrails, Pacman.NoPelletsLeft)
                End If
                If (StartRenderX <= OrangeGhost.IX <= EndRenderX) And (StartRenderY <= OrangeGhost.IY <= EndRenderY) Then
                    OrangeGhost.DrawOverPreviousLocation(GFX, IntTileSize, Maps(MapNo), PaintTrails, Pacman.NoPelletsLeft)
                End If
                If (StartRenderX <= OrangeGhost.IX <= EndRenderX) And (StartRenderY <= OrangeGhost.IY <= EndRenderY) Then
                    BlueGhost.DrawOverPreviousLocation(GFX, IntTileSize, Maps(MapNo), PaintTrails, Pacman.NoPelletsLeft)
                End If
            End If


            If Pacman.NoPelletsLeft = True Then

                Pacman.DrawOverPreviousLocation(GFX, IntTileSize, Maps(MapNo), PaintTrails, Pacman.NoPelletsLeft)
                RedGhost.DrawOverPreviousLocation(GFX, IntTileSize, Maps(MapNo), PaintTrails, Pacman.NoPelletsLeft)
                PinkGhost.DrawOverPreviousLocation(GFX, IntTileSize, Maps(MapNo), PaintTrails, Pacman.NoPelletsLeft)
                OrangeGhost.DrawOverPreviousLocation(GFX, IntTileSize, Maps(MapNo), PaintTrails, Pacman.NoPelletsLeft)
                BlueGhost.DrawOverPreviousLocation(GFX, IntTileSize, Maps(MapNo), PaintTrails, Pacman.NoPelletsLeft)

                If PaintTrails = False Then
                    Pacman.DrawOverPreviousLocation(GFX, IntTileSize, Maps(MapNo), PaintTrails, Pacman.NoPelletsLeft)
                End If
                Pacman.Draw(GFX, IntTileSize)

                'Restes Pacmans location 
                Pacman.IX = 13
                Pacman.IY = 23
                Pacman.BackTileX = 13
                Pacman.BackTileY = 23

            Else

                If (Pacman.IX = 13 And Pacman.IY = 23 And (Pacman.Direction = 0 Or GameReset = True)) Or (Disco = False And LightsOut = True) Then

                    'Fix overdraw BackBuffer
                    GFX.Clear(Color.Black)

                    'Draw based tiles for map array in play
                    For x = StartRenderX To EndRenderX
                        For y = StartRenderY To EndRenderY
                            r = New Rectangle(x * IntTileSize, y * IntTileSize, IntTileSize, IntTileSize)

                            'Find tiles and draws
                            Select Case Maps(MapNo)(y, x)
                                Case 0 'Empty space
                                    GFX.FillRectangle(Brushes.Black, r)
                                    GFX.DrawRectangle(Pens.Black, r)
                                Case 1 'Pellet
                                    GFX.FillRectangle(Brushes.Black, r)
                                        Dim pelletRect As New Rectangle(r.X + IntTileSize \ 4, r.Y + IntTileSize \ 4, IntTileSize \ 2, IntTileSize \ 2)
                                    If BlackAndWhite = True Then
                                        GFX.FillEllipse(Brushes.Gray, pelletRect)
                                    Else
                                        GFX.FillEllipse(Brushes.Yellow, pelletRect)
                                    End If
                                    GFX.DrawRectangle(Pens.Black, r)
                                Case 2 'Wall
                                    If BlackAndWhite = True Then
                                        GFX.FillRectangle(Brushes.White, r)
                                    Else
                                        GFX.FillRectangle(Brushes.Blue, r)
                                    End If
                                    GFX.DrawRectangle(Pens.Black, r)
                                Case -1 'Pellet portal
                                    If BlackAndWhite = True Then
                                        GFX.FillRectangle(Brushes.DarkGray, r)
                                    Else
                                        GFX.FillRectangle(Brushes.Red, r)
                                    End If
                                    Dim pelletRect As New Rectangle(r.X + IntTileSize \ 4, r.Y + IntTileSize \ 4, IntTileSize \ 2, IntTileSize \ 2)
                                    If BlackAndWhite = True Then
                                        GFX.FillEllipse(Brushes.Gray, pelletRect)
                                    Else
                                        GFX.FillEllipse(Brushes.Yellow, pelletRect)
                                    End If
                                    GFX.DrawRectangle(Pens.Black, r)
                                Case -2 'Empty portal 
                                    If BlackAndWhite = True Then
                                        GFX.FillRectangle(Brushes.DarkGray, r)
                                    Else
                                        GFX.FillRectangle(Brushes.Red, r)
                                    End If
                                    GFX.DrawRectangle(Pens.Black, r)
                                Case 3 'Mega Pellet 
                                    GFX.FillRectangle(Brushes.Black, r)
                                    Dim pelletRect As New Rectangle(r.X + IntTileSize \ 8, r.Y + IntTileSize \ 8, 4 * IntTileSize / 5, 4 * IntTileSize / 5)
                                    If BlackAndWhite = True Then
                                        GFX.FillEllipse(Brushes.Gray, pelletRect)
                                    Else
                                        GFX.FillEllipse(Brushes.Yellow, pelletRect)
                                    End If
                                    GFX.DrawRectangle(Pens.Black, r)
                                Case 4 'Ghost spawn
                                    GFX.FillRectangle(Brushes.Black, r)
                                    GFX.DrawRectangle(Pens.Black, r)
                                Case 5 'Ghost gate 
                                    GFX.FillRectangle(Brushes.gray, r)
                                    GFX.DrawRectangle(Pens.Black, r)
                            End Select

                        Next
                    Next

                ElseIf Disco = True Then

                    If DiscoCount = 1 Then

                        DiscoCount = 0

                        'Fix overdraw BackBuffer
                        GFX.Clear(Color.Black)


                        'Draw based tiles for map array with random colours
                        For x = StartRenderX To EndRenderX
                            For y = StartRenderY To EndRenderY
                                r = New Rectangle(x * IntTileSize, y * IntTileSize, IntTileSize, IntTileSize)

                                'Find tiles and draws
                                Select Case Maps(MapNo)(y, x)
                                    Case 0 'Empty space
                                        GFX.FillRectangle(RandomDarkColour(), r)
                                        GFX.DrawRectangle(Pens.Black, r)
                                        If PaintTrails = True AndAlso DiscoTrailMaps(MapNo)(y, x) = 0 Then
                                            Dim FillGhostTrail As New Rectangle(x * IntTileSize, y * IntTileSize, IntTileSize, IntTileSize)
                                            GFX.FillEllipse(RandomDarkColour(), r)
                                        End If
                                    Case 1 'Pellet
                                        GFX.FillRectangle(RandomDarkColour(), r)
                                        If PaintTrails = True AndAlso DiscoTrailMaps(MapNo)(y, x) = 0 Then
                                            Dim FillGhostTrail As New Rectangle(x * IntTileSize, y * IntTileSize, IntTileSize, IntTileSize)
                                            GFX.FillEllipse(RandomDarkColour(), r)
                                        End If
                                        Dim pelletRect As New Rectangle(r.X + IntTileSize \ 4, r.Y + IntTileSize \ 4, IntTileSize \ 2, IntTileSize \ 2)
                                        If BlackAndWhite = True Then
                                            GFX.FillEllipse(Brushes.Gray, pelletRect)
                                        Else
                                            GFX.FillEllipse(Brushes.Yellow, pelletRect)
                                        End If
                                        GFX.DrawRectangle(Pens.Black, r)
                                    Case 2 'Wall
                                        GFX.FillRectangle(RandomLightColour(), r)
                                        GFX.DrawRectangle(Pens.Black, r)
                                    Case -1 'Pellet portal
                                        GFX.FillRectangle(RandomDarkColour(), r)
                                        If PaintTrails = True AndAlso DiscoTrailMaps(MapNo)(y, x) = 0 Then
                                            Dim FillGhostTrail As New Rectangle(x * IntTileSize, y * IntTileSize, IntTileSize, IntTileSize)
                                            GFX.FillEllipse(RandomDarkColour(), r)
                                        End If
                                        Dim pelletRect As New Rectangle(r.X + IntTileSize \ 4, r.Y + IntTileSize \ 4, IntTileSize \ 2, IntTileSize \ 2)
                                        If BlackAndWhite = True Then
                                            GFX.FillEllipse(Brushes.Gray, pelletRect)
                                        Else
                                            GFX.FillEllipse(Brushes.Yellow, pelletRect)
                                        End If
                                        GFX.DrawRectangle(Pens.Black, r)
                                    Case -2 'Empty portal 
                                        GFX.FillRectangle(RandomDarkColour(), r)
                                        GFX.DrawRectangle(Pens.Black, r)
                                        If PaintTrails = True AndAlso DiscoTrailMaps(MapNo)(y, x) = 0 Then
                                            Dim FillGhostTrail As New Rectangle(x * IntTileSize, y * IntTileSize, IntTileSize, IntTileSize)
                                            GFX.FillEllipse(RandomDarkColour(), r)
                                        End If
                                    Case 3 'Mega Pellet 
                                        GFX.FillRectangle(RandomDarkColour(), r)
                                        If PaintTrails = True AndAlso DiscoTrailMaps(MapNo)(y, x) = 0 Then
                                            Dim FillGhostTrail As New Rectangle(x * IntTileSize, y * IntTileSize, IntTileSize, IntTileSize)
                                            GFX.FillEllipse(RandomDarkColour(), r)
                                        End If
                                        Dim pelletRect As New Rectangle(r.X + IntTileSize \ 8, r.Y + IntTileSize \ 8, 4 * IntTileSize / 5, 4 * IntTileSize / 5)
                                        If BlackAndWhite = True Then
                                            GFX.FillEllipse(Brushes.Gray, pelletRect)
                                        Else
                                            GFX.FillEllipse(Brushes.Yellow, pelletRect)
                                        End If
                                        GFX.DrawRectangle(Pens.Black, r)
                                    Case 4 'Ghost spawn
                                        GFX.FillRectangle(RandomDarkColour(), r)
                                        GFX.DrawRectangle(Pens.Black, r)
                                        If PaintTrails = True AndAlso DiscoTrailMaps(MapNo)(y, x) = 0 Then
                                            Dim FillGhostTrail As New Rectangle(x * IntTileSize, y * IntTileSize, IntTileSize, IntTileSize)
                                            GFX.FillEllipse(RandomDarkColour(), r)
                                        End If
                                    Case 5 'Ghost gate 
                                        GFX.FillRectangle(RandomLightColour(), r)
                                        GFX.DrawRectangle(Pens.Black, r)
                                        If PaintTrails = True AndAlso DiscoTrailMaps(MapNo)(y, x) = 0 Then
                                            Dim FillGhostTrail As New Rectangle(x * IntTileSize, y * IntTileSize, IntTileSize, IntTileSize)
                                            GFX.FillEllipse(RandomDarkColour(), r)
                                        End If
                                End Select

                            Next
                        Next

                    Else
                        DiscoCount = 1
                    End If

                End If

                'Draws Pac-Man
                Pacman.Draw(GFX, IntTileSize)

                'Draws Ghosts 
                If (StartRenderX <= RedGhost.IX And RedGhost.IX <= EndRenderX) And (StartRenderY <= RedGhost.IY And RedGhost.IY <= EndRenderY) Then
                    RedGhost.Draw(GFX, IntTileSize)
                End If
                If (StartRenderX <= PinkGhost.IX And PinkGhost.IX <= EndRenderX) And (StartRenderY <= PinkGhost.IY And PinkGhost.IY <= EndRenderY) Then
                    PinkGhost.Draw(GFX, IntTileSize)
                End If
                If (StartRenderX <= OrangeGhost.IX And OrangeGhost.IX <= EndRenderX) And (StartRenderY <= OrangeGhost.IY And OrangeGhost.IY <= EndRenderY) Then
                    OrangeGhost.Draw(GFX, IntTileSize)
                End If
                If (StartRenderX <= BlueGhost.IX And BlueGhost.IX <= EndRenderX) And (StartRenderY <= BlueGhost.IY And BlueGhost.IY <= EndRenderY) Then
                    BlueGhost.Draw(GFX, IntTileSize)
                End If

            End If

        End Using

        'Draws BackBuffer to panel
        Using BackBufferGFX As Graphics = MapDisplay1.CreateGraphics
            BackBufferGFX.DrawImage(BackBuffer, 0, 0, IntResWidth, IntResHeight)
        End Using

        If Pacman.NoPelletsLeft = True Then
            Pacman.NoPelletsLeft = False
            System.Threading.Thread.Sleep(2000) 'Wait 2 seconds
            GameReset = True
        End If

    End Sub

#Region "Random Colour Generators"
    Public Function RandomDarkColour() As Brush

        Dim rand As New Random()
        Dim randomNumber As Integer

        Dim ReturnColor As Color

        If BlackAndWhite = False Then

            Dim ColourArray() As Color =
        {Color.Black, Color.Maroon, Color.Green, Color.Navy, Color.DarkRed,
        Color.Brown, Color.DarkGreen, Color.DarkOliveGreen, Color.DarkSlateGray,
        Color.Indigo, Color.MidnightBlue, Color.SaddleBrown, Color.Sienna,
        Color.Chocolate, Color.Firebrick, Color.DarkGoldenrod, Color.DarkSlateBlue,
        Color.DarkBlue, Color.Teal, Color.Olive, Color.SteelBlue, Color.Peru,
        Color.SlateGray, Color.DimGray, Color.Gray}

            randomNumber = rand.Next(1, ColourArray.Count - 1)

            ReturnColor = (ColourArray(randomNumber))

        Else

            Dim LightColourArray() As Color =
        {Color.White, Color.Gainsboro, Color.LightGray, Color.Silver,
         Color.WhiteSmoke, Color.FloralWhite, Color.OldLace, Color.SeaShell,
         Color.Linen, Color.Beige, Color.MistyRose, Color.AntiqueWhite,
         Color.Snow, Color.Honeydew, Color.Azure, Color.LavenderBlush,
         Color.Ivory, Color.Lavender, Color.LightCyan, Color.LightYellow}

            randomNumber = rand.Next(1, LightColourArray.Count - 1)

            ReturnColor = (LightColourArray(randomNumber))

        End If

        Return New SolidBrush(ReturnColor)

    End Function

    Private Function RandomLightColour() As Brush

        Dim rnd As New Random()
        Dim ReturnColor As Color

        If BlackAndWhite = False Then

            ReturnColor = Color.FromArgb(rnd.Next(120, 240), rnd.Next(120, 240), rnd.Next(120, 240))

        Else 'This outputs a dark colour for the white and black mode (this is a developer choice)

            Dim DarkColourArray() As Color =
        {Color.Black, Color.DimGray, Color.Gray, Color.DarkGray,
         Color.SlateGray, Color.DarkSlateGray, Color.LightSlateGray}

            Dim randomNumber As Integer = rnd.Next(1, DarkColourArray.Count - 1)

            ReturnColor = (DarkColourArray(randomNumber))

        End If

        Return New SolidBrush(ReturnColor)

    End Function

#End Region

#End Region

#Region "Label and System Update/Information"

    Private Sub UpdateLocalTickCounterAndFPS() 'Updates FPS and Time 

        If InttSeconds = TimeOfDay.Second And BoolIsRunning = True Then
            InttTicks += 1
        Else
            IntMaxTicks = InttTicks
            InttSeconds = TimeOfDay.Second
            InttTicks = 0
        End If

    End Sub
    Private Sub UpdateLabelsDisplay()

        If BoolIsRunning = True Then

            'Updates/Displays current game stats and performance
            lblMaxtick.Text = "FPS: " & IntMaxTicks 'FPS
            lbltTick.Text = "Ticks: " & InttTicks 'Tick speed
            lblPacmanlocation.Text = "Pacman (x,y): (" & Pacman.IX & "," & Pacman.IY & ")" 'Pacman coordinates
            Lblscore.Text = "Score: " & Pacman.Score 'Score
            lblFrighten.Text = "Ghost Unfrighten In: " & IntGhostUnFrightenCountdown

            'Updates/Displays the time
            If IsTimedlevel = True Then
                lbltimer.Text = "time: " & 250 - IntTime 'Time left 
            ElseIf IsTimedlevel = False Then
                lbltimer.Text = "time: " & IntTime 'Time left 
            End If

            'Updates/Displays the current direction Pacman is facing
            If Pacman.Direction = 37 Then
                lblDirection.Text = "Direction: Left"
            ElseIf Pacman.Direction = 38 Then
                lblDirection.Text = "Direction: Up"
            ElseIf Pacman.Direction = 39 Then
                lblDirection.Text = "Direction: Right"
            ElseIf Pacman.Direction = 40 Then
                lblDirection.Text = "Direction: Down"
            Else
                lblDirection.Text = "Direction: Unknown"
            End If

            'Updates/Displays the direction Pacman will go next
            If Pacman.NextDirection = 37 Then
                lblNextDirection.Text = "NextDirection: Left"
            ElseIf Pacman.NextDirection = 38 Then
                lblNextDirection.Text = "NextDirection: Up"
            ElseIf Pacman.NextDirection = 39 Then
                lblNextDirection.Text = "NextDirection: Right"
            ElseIf Pacman.NextDirection = 40 Then
                lblNextDirection.Text = "NextDirection: Down"
            Else
                lblNextDirection.Text = "NextDirection: Unknown"
            End If

        End If

    End Sub

#End Region

#Region "Game/System Timers"
    Private Sub Control_Tick(sender As Object, e As EventArgs) Handles Control.Tick

        If Gameover = True Or (IsTimedlevel = True And IntTime > 250) Then

#Region "Gameover Stages"

            'Stops Game and displays stats

            Dim score = Pacman.Score

            'End game stages
            'These stages ensure that the game stats are loaded successfully
            'They are as follows:
            If GameoverStage = 0 Then 'Stage 1 = Stop the main game running which halts display and pathfinding prossessing 

                BoolIsRunning = False
                PnlGameStats.Visible = True
                lblGameOver.Visible = True

                GameoverStage += 1

            ElseIf GameoverStage = 1 Then 'Stage 2 = displays all player stats of that game 

                'Sets the final scores 
                'Opens the panels to display the final game stats

                lblFinalTime.Text = lbltimer.Text
                Lblfinalscore.Text = Lblscore.Text


                'Saves the score player achived to a node pad so that the socre can be used for the global scores on the stats page

                If Not score < 1 Then

                    'Declares a location for the file path to be stored
                    Dim filePath As String = Nothing

                    'Finds correct file path to store in the correct location
                    Select Case MapNo
                        Case 0
                            If IsTimedlevel = True Then
                                filePath = Application.StartupPath & "GlobalTimedScorseMap1.txt"
                            Else
                                filePath = Application.StartupPath & "GlobalUntimedScorseMap1.txt"
                            End If
                        Case 1
                            If IsTimedlevel = True Then
                                filePath = Application.StartupPath & "GlobalTimedScorseMap2.txt"
                            Else
                                filePath = Application.StartupPath & "GlobalUntimedScorseMap2.txt"
                            End If
                    End Select

                    'Saves the score to the correct file
                    File.AppendAllText(filePath, score.ToString() & Environment.NewLine)

                End If

                'Saves the score achived to the correct local list

                If IsTimedlevel = True Then
                    If MapNo = 0 Then
                        Form1.ScoresMapATimed.Add(score)
                    ElseIf MapNo = 1 Then
                        Form1.ScoresMapBTimed.Add(score)
                    End If
                ElseIf IsTimedlevel = False Then
                    If MapNo = 0 Then
                        Form1.ScoresMapAUntimed.Add(score)
                    ElseIf MapNo = 1 Then
                        Form1.ScoresMapBuntimed.Add(score)
                    End If
                End If

                GameoverStage += 1

            ElseIf GameoverStage = 2 Then 'Stage 3 = Halts program for 5 seconds and then exits to main menu 

                System.Threading.Thread.Sleep(5000) 'Wait 5 seconds 

                PnlGameOptions.Visible = True
                Timer.Enabled = False
                GhostTimer.Enabled = False
                Control.Enabled = False
                GhostActivationTimer.Enabled = False

                GameoverStage = 0

                Form1.Visible = True
                Me.Close()

            End If

#End Region

            'All the below variables combined reperesent the start state of the game 
            'This cannot be met after the player has played a single move 
            'Unless the player eats all the pellets resetting the map 
        ElseIf Pacman.Direction = 0 Or GameReset = True Then

#Region "Game Initiation/Reset"

            GhostActivationTimer.Enabled = True
            GhostActivationCount = 0

            'Resets 
            If GhostTimer.Enabled = False Then
                GhostTimer.Enabled = True
            End If

            PinkGhost.IsActive = False
            BlueGhost.IsActive = False
            OrangeGhost.IsActive = False


            'Resets all ghosts into start locations
            RedGhost.IX = 13
            RedGhost.IY = 11
            PinkGhost.IX = 12
            PinkGhost.IY = 14
            OrangeGhost.IX = 13
            OrangeGhost.IY = 14
            BlueGhost.IX = 14
            BlueGhost.IY = 14

            'Resets all Mega pellets into start locations respecting the map No.
            If MapNo = 1 Then
                Maps(MapNo)(3, 1) = 3
                Maps(MapNo)(3, 26) = 3
                Maps(MapNo)(21, 1) = 3
                Maps(MapNo)(21, 26) = 3
            End If
            If MapNo = 0 Then
                Maps(MapNo)(3, 1) = 3
                Maps(MapNo)(3, 26) = 3
                Maps(MapNo)(23, 1) = 3
                Maps(MapNo)(23, 26) = 3
            End If

            'Resets all Ghost pathfinding paths
            RedGhost.GhostPath = Nothing
            PinkGhost.GhostPath = Nothing
            OrangeGhost.GhostPath = Nothing
            BlueGhost.GhostPath = Nothing

            'Ghosts cant be in frightened mode when starting the game
            RedGhost.frightened = False
            PinkGhost.frightened = False
            OrangeGhost.frightened = False
            BlueGhost.frightened = False

            'The red Ghost is the only Ghost active right away 
            RedGhost.IsActive = True

            'These Previous node locations allow the Ghost to move in the correct direction at the start of the game
            'This direction being left 
            RedGhost.PreviousNodeX = 15
            RedGhost.PreviousNodeY = 11
            RedGhost.PreviousTileX = 14
            RedGhost.PreviousTileY = 11

            'All other ghosts are inactive until they get activated in the Ghost timer
            PinkGhost.IsActive = False
            OrangeGhost.IsActive = False
            BlueGhost.IsActive = False

            'Resets the Ghost activation count and enables the Ghost timer
            GhostActivationCount = 0
            GhostActivationTimer.Enabled = True
            Modecount = 0
            If Speedx10 = True Then
                ScatterTimeCountTotal = 2
                ChaseTimeCountTotal = 7
            Else
                ScatterTimeCountTotal = 7
                ChaseTimeCountTotal = 20
            End If

            'Scatter mode is the fist mode the ghosts will be in at the start of the game
            'This is why scatter mode is active
            ModeScatter = True

            'Informs the player of how to start the game
            lblControls.Text = "Press arrow keys 
to start"

            'Draws the graphical map onto the screen
            DrawGraphics()

            Pacman.GameSpeedIncreaseDueToPelletCount = False
            GameReset = False

#End Region

        ElseIf BoolIsRunning = True Then

#Region "Main Game Code"

            'Resets 
            If GhostTimer.Enabled = False Then
                GhostTimer.Enabled = True
            End If

            lblControls.Text = "Controls"

            'Keep app responsive 
            Application.DoEvents()

            'Move Pac-Man
            If Speedx10 = True Then
                Pacman.Mspeed = 2
            End If

            Pacman.Move(Maps(MapNo))

            DiscoTrailMaps(MapNo)(Pacman.IY, Pacman.IX) = 0

            'Ghosts change into frightened mode 
            If Pacman.MegaPelletEaten = True Then

                Pacman.MegaPelletEaten = False
                IsGhostsFrightened = True
                FrightenedGhostcount = 0

                'Sets Frighten user interface timer for the Ghost
                '180D turn if changing modes (If ghosts already in frighened they dont change)

#Region "Changes Ghost state into Frightened"

                IntGhostUnFrightenCountdown = GhostFrightenedTime

                'Change all ghosts mode to frightened 

                'If all criteria is met the ghosts will change into frightened state
                'if Ghost is active and if the Ghost is not in the Ghost box then the Ghost will change into frightened state

                If RedGhost.IsActive = True And
                    (Not (RedGhost.IX = 13 And (RedGhost.IY = 12 Or RedGhost.IY = 13 Or RedGhost.IY = 14))) And
                    (Not (RedGhost.IY = 14 And (RedGhost.IX = 12 Or RedGhost.IX = 14 Or RedGhost.IX = 15))) Then
                    If (RedGhost.PreviousTileX = 13 And RedGhost.PreviousTileY = 12) Then
                        RedGhost.PreviousTileX = 14
                        RedGhost.PreviousTileY = 11
                    End If
                    If RedGhost.frightened = False Then
                        RedGhost.changing = True
                    End If
                    RedGhost.frightened = True
                End If

                If PinkGhost.IsActive = True And
                    (Not (PinkGhost.IX = 13 And (PinkGhost.IY = 12 Or PinkGhost.IY = 13 Or PinkGhost.IY = 14))) And
                    (Not (PinkGhost.IY = 14 And (PinkGhost.IX = 12 Or PinkGhost.IX = 14 Or PinkGhost.IX = 15))) Then
                    If (PinkGhost.PreviousTileX = 13 And PinkGhost.PreviousTileY = 12) Then
                        PinkGhost.PreviousTileX = 14
                        PinkGhost.PreviousTileY = 11
                    End If
                    If PinkGhost.frightened = False Then
                        PinkGhost.changing = True
                    End If
                    PinkGhost.frightened = True
                End If

                If OrangeGhost.IsActive = True And
                    (Not (OrangeGhost.IX = 13 And (OrangeGhost.IY = 12 Or OrangeGhost.IY = 13 Or OrangeGhost.IY = 14))) And
                    (Not (OrangeGhost.IY = 14 And (OrangeGhost.IX = 12 Or OrangeGhost.IX = 14 Or OrangeGhost.IX = 15))) Then
                    If (OrangeGhost.PreviousTileX = 13 And OrangeGhost.PreviousTileY = 12) Then
                        OrangeGhost.PreviousTileX = 14
                        OrangeGhost.PreviousTileY = 11
                    End If
                    If OrangeGhost.frightened = False Then
                        OrangeGhost.changing = True
                    End If
                    OrangeGhost.frightened = True
                End If

                If BlueGhost.IsActive = True And
                    (Not (BlueGhost.IX = 13 And (BlueGhost.IY = 12 Or BlueGhost.IY = 13 Or BlueGhost.IY = 14))) And
                    (Not (BlueGhost.IY = 14 And (BlueGhost.IX = 12 Or BlueGhost.IX = 14 Or BlueGhost.IX = 15))) Then
                    If (BlueGhost.PreviousTileX = 13 And BlueGhost.PreviousTileY = 12) Then
                        BlueGhost.PreviousTileX = 14
                        BlueGhost.PreviousTileY = 11
                    End If
                    If BlueGhost.frightened = False Then
                        BlueGhost.changing = True
                    End If
                    BlueGhost.frightened = True
                End If

#End Region

            End If

#Region "Ghost Movement Execution"

            'Types of Ghost speeds
            Dim SlowGhostSpeed As Integer 'Speed for when the ghost is frightened 
            Dim NormalGhostSpeed As Integer  'Base speed when the ghost is either in chace or scatter
            Dim FastGhostSpeed As Integer  'Speed for when the ghost has been eaten

            If Speedx10 = True Then
                SlowGhostSpeed = 2
                NormalGhostSpeed = 1
                FastGhostSpeed = 0
            Else

                SlowGhostSpeed = 20
                NormalGhostSpeed = 5
                FastGhostSpeed = 4
            End If

            If Pacman.GameSpeedIncreaseDueToPelletCount = True Then
                SlowGhostSpeed -= 5
                NormalGhostSpeed -= 1
                FastGhostSpeed -= 1
            End If

            'Calculating the ghosts next tile and path
            'By updating their knowlage of pacmans location and other variables
            If Not Pacman.Direction = 0 And Not Pacman.NextDirection = 0 And Not GameReset = True Then
                If (RedGhost.frightened = False And RedGhost.GhostSpeedCount >= NormalGhostSpeed) Or (RedGhost.frightened = True And RedGhost.GhostSpeedCount >= SlowGhostSpeed) Or (RedGhost.Eaten = True And RedGhost.GhostSpeedCount >= 2) Then
                    RedGhost.TrackingIX = Pacman.IX
                    RedGhost.TrackingIY = Pacman.IY
                    DiscoTrailMaps(MapNo)(RedGhost.IY, RedGhost.IX) = 0
                    RedGhost.Move(Maps(MapNo))

                    'Updates red ghosts position for Blue ghosts pathfinding personality
                    BlueGhost.RedghostIX = RedGhost.IX
                    BlueGhost.RedghostIY = RedGhost.IY

                    RedGhost.GhostSpeedCount = 0
                Else
                    RedGhost.GhostSpeedCount += 1
                End If

                If (PinkGhost.frightened = False And PinkGhost.GhostSpeedCount >= NormalGhostSpeed) Or (PinkGhost.frightened = True And PinkGhost.GhostSpeedCount >= SlowGhostSpeed) Or (PinkGhost.Eaten = True And PinkGhost.GhostSpeedCount >= 2) Then
                    PinkGhost.TrackingIX = Pacman.IX
                    PinkGhost.TrackingIY = Pacman.IY

                    'Updates Pacman Direction for Pink ghosts pathfinding personality
                    PinkGhost.PacmanDirection = Pacman.Direction

                    DiscoTrailMaps(MapNo)(PinkGhost.IY, PinkGhost.IX) = 0

                    PinkGhost.Move(Maps(MapNo))
                    PinkGhost.GhostSpeedCount = 0
                Else
                    PinkGhost.GhostSpeedCount += 1
                End If

                If (BlueGhost.frightened = False And BlueGhost.GhostSpeedCount >= NormalGhostSpeed) Or (BlueGhost.frightened = True And BlueGhost.GhostSpeedCount >= SlowGhostSpeed) Or (BlueGhost.Eaten = True And BlueGhost.GhostSpeedCount >= 2) Then
                    BlueGhost.TrackingIX = Pacman.IX
                    BlueGhost.TrackingIY = Pacman.IY

                    'Updates Pacman Direction for Blue ghosts pathfinding personality
                    BlueGhost.PacmanDirection = Pacman.Direction

                    DiscoTrailMaps(MapNo)(BlueGhost.IY, BlueGhost.IX) = 0

                    BlueGhost.Move(Maps(MapNo))
                    BlueGhost.GhostSpeedCount = 0
                Else
                    BlueGhost.GhostSpeedCount += 1
                End If

                If (OrangeGhost.frightened = False And OrangeGhost.GhostSpeedCount >= NormalGhostSpeed) Or (OrangeGhost.frightened = True And OrangeGhost.GhostSpeedCount >= SlowGhostSpeed) Or (OrangeGhost.Eaten = True And OrangeGhost.GhostSpeedCount >= 2) Then
                    OrangeGhost.TrackingIX = Pacman.IX
                    OrangeGhost.TrackingIY = Pacman.IY

                    DiscoTrailMaps(MapNo)(OrangeGhost.IY, OrangeGhost.IX) = 0

                    OrangeGhost.Move(Maps(MapNo))
                    OrangeGhost.GhostSpeedCount = 0
                Else
                    OrangeGhost.GhostSpeedCount += 1
                End If
            End If

#End Region

            'Draw Graphics  
            DrawGraphics()

#Region "Check Pacman Ghost Collision"

            'Checks if any ghosts shares the same coordinates as Pacman 
            'If they do one of two things can happen 
            'Either if Ghost is in frightened mode the Ghost will be eaten and Pacman will gain 200 score
            'Or
            'If Ghost is not frightened the game will end as the Ghost has cought Pacman

            'The first boolean part is to test if Pacman and the ghosts are in the same location therefore the game is over 
            'The second boolean part is to test a very spacific frame perfect position where the a ghost and Pacman have gone through eachover
            If (RedGhost.IX = Pacman.IX And RedGhost.IY = Pacman.IY And RedGhost.IsActive = True) Or
                (RedGhost.PreviousTileX = Pacman.IX And RedGhost.PreviousTileY = Pacman.IY And RedGhost.IsActive = True And
                ((RedGhost.Direction = 37 And Pacman.Direction = 39) Or (RedGhost.Direction = 39 And Pacman.Direction = 37) Or
                (RedGhost.Direction = 38 And Pacman.Direction = 40) Or (RedGhost.Direction = 40 And Pacman.Direction = 38))) Then

                If (RedGhost.Eaten = False And RedGhost.frightened = False) Then
                    Gameover = True
                Else
                    Pacman.Score += 200
                    RedGhost.Notchanging = True
                    RedGhost.Eaten = True
                    If RedGhost.IX = 13 Then
                        Select Case RedGhost.IY
                            Case 13
                                RedGhost.Eaten = False
                            Case 12
                                RedGhost.Eaten = False
                            Case 11
                                RedGhost.Eaten = False
                        End Select
                    End If
                End If
            End If

            If (PinkGhost.IX = Pacman.IX And PinkGhost.IY = Pacman.IY And PinkGhost.IsActive = True) Or
                (PinkGhost.PreviousTileX = Pacman.IX And PinkGhost.PreviousTileY = Pacman.IY And PinkGhost.IsActive = True And
                ((PinkGhost.Direction = 37 And Pacman.Direction = 39) Or (PinkGhost.Direction = 39 And Pacman.Direction = 37) Or
                (PinkGhost.Direction = 38 And Pacman.Direction = 40) Or (PinkGhost.Direction = 40 And Pacman.Direction = 38))) Then

                If (PinkGhost.Eaten = False And PinkGhost.frightened = False) Then
                    Gameover = True
                Else
                    Pacman.Score += 200
                    PinkGhost.Notchanging = True
                    PinkGhost.Eaten = True

                    If PinkGhost.IX = 13 Then
                        Select Case PinkGhost.IY
                            Case 13
                                PinkGhost.Eaten = False
                            Case 12
                                PinkGhost.Eaten = False
                            Case 11
                                PinkGhost.Eaten = False
                        End Select
                    End If

                End If

            End If

            If (BlueGhost.IX = Pacman.IX And BlueGhost.IY = Pacman.IY And BlueGhost.IsActive = True) Or
                (BlueGhost.PreviousTileX = Pacman.IX And BlueGhost.PreviousTileY = Pacman.IY And BlueGhost.IsActive = True And
                ((BlueGhost.Direction = 37 And Pacman.Direction = 39) Or (BlueGhost.Direction = 39 And Pacman.Direction = 37) Or
                (BlueGhost.Direction = 38 And Pacman.Direction = 40) Or (BlueGhost.Direction = 40 And Pacman.Direction = 38))) Then

                If (BlueGhost.Eaten = False And BlueGhost.frightened = False) Then
                    Gameover = True
                Else
                    Pacman.Score += 200
                    BlueGhost.Notchanging = True
                    BlueGhost.Eaten = True
                    If BlueGhost.IX = 13 Then
                        Select Case BlueGhost.IY
                            Case 13
                                BlueGhost.Eaten = False
                            Case 12
                                BlueGhost.Eaten = False
                            Case 11
                                BlueGhost.Eaten = False
                        End Select
                    End If
                End If
            End If

            If (OrangeGhost.IX = Pacman.IX And OrangeGhost.IY = Pacman.IY And OrangeGhost.IsActive = True) Or
                (OrangeGhost.PreviousTileX = Pacman.IX And OrangeGhost.PreviousTileY = Pacman.IY And OrangeGhost.IsActive = True And
                ((OrangeGhost.Direction = 37 And Pacman.Direction = 39) Or (OrangeGhost.Direction = 39 And Pacman.Direction = 37) Or
                (OrangeGhost.Direction = 38 And Pacman.Direction = 40) Or (OrangeGhost.Direction = 40 And Pacman.Direction = 38))) Then

                If (OrangeGhost.Eaten = False And OrangeGhost.frightened = False) Then
                    Gameover = True
                Else
                    Pacman.Score += 200
                    OrangeGhost.Notchanging = True
                    OrangeGhost.Eaten = True
                    If OrangeGhost.IX = 13 Then
                        Select Case OrangeGhost.IY
                            Case 13
                                OrangeGhost.Eaten = False
                            Case 12
                                OrangeGhost.Eaten = False
                            Case 11
                                OrangeGhost.Eaten = False
                        End Select
                    End If
                End If
            End If

#End Region

            'Add sound and effects (future)

            'Updates FPS/Time 
            UpdateLocalTickCounterAndFPS()
            UpdateLabelsDisplay()

#End Region

        Else

            'When game is not running disable the Ghost timer
            'This stops ghosts from moving 

            If GhostTimer.Enabled = True Then
                GhostTimer.Enabled = False
            End If

        End If

    End Sub
    Private Sub GhostTimer_Tick(sender As Object, e As EventArgs) Handles GhostTimer.Tick

        'Ghosts  for modes
        If Not IsGhostsFrightened = True Then 'When Ghosts are not Frightened

            If ModeScatter = True Then
                Modecount += 1
                If Modecount = ScatterTimeCountTotal Then
                    'Changing the mode to Chase

                    RedGhost.Scatter = False
                    PinkGhost.Scatter = False
                    OrangeGhost.Scatter = False
                    BlueGhost.Scatter = False
                    ModeScatter = False

                    'Ghosts reverse direction (180° turn) 
                    If RedGhost.IsActive = True Then
                        RedGhost.changing = True
                    End If
                    If PinkGhost.IsActive = True Then
                        PinkGhost.changing = True
                    End If
                    If OrangeGhost.IsActive = True Then
                        OrangeGhost.changing = True
                    End If
                    If BlueGhost.IsActive = True Then
                        BlueGhost.changing = True
                    End If

                    Modecount = 0
                End If
            Else
                Modecount += 1
                If Modecount = ChaseTimeCountTotal Then
                    'Changing the mode to scatter 
                    RedGhost.Scatter = True
                    PinkGhost.Scatter = True
                    OrangeGhost.Scatter = True
                    BlueGhost.Scatter = True
                    ModeScatter = True
                    'Ghosts reverse direction (180° turn) 
                    If RedGhost.IsActive = True Then
                        RedGhost.changing = True
                    End If
                    If PinkGhost.IsActive = True Then
                        PinkGhost.changing = True
                    End If
                    If OrangeGhost.IsActive = True Then
                        OrangeGhost.changing = True
                    End If
                    If BlueGhost.IsActive = True Then
                        BlueGhost.changing = True
                    End If
                    Modecount = 0
                End If
            End If
        ElseIf IsGhostsFrightened = True Then
            If FrightenedGhostcount = GhostFrightenedTime Then
                'Changing the mode to the mode before frightened 
                IsGhostsFrightened = False
                RedGhost.frightened = False
                PinkGhost.frightened = False
                OrangeGhost.frightened = False
                BlueGhost.frightened = False

                'Ghosts reverse direction (180° turn) 

                If RedGhost.IsActive = True Then
                    RedGhost.changing = True
                End If
                If PinkGhost.IsActive = True Then
                    PinkGhost.changing = True
                End If
                If OrangeGhost.IsActive = True Then
                    OrangeGhost.changing = True
                End If
                If BlueGhost.IsActive = True Then
                    BlueGhost.changing = True
                End If

                IntGhostUnFrightenCountdown = GhostFrightenedTime
                FrightenedGhostcount = 0
            End If

            If FrightenedGhostcount = 0 Then
                lblFrighten.Visible = False
            Else
                lblFrighten.Visible = True
            End If

            FrightenedGhostcount += 1
            IntGhostUnFrightenCountdown -= 1

        End If


    End Sub
    Private Sub Timer_Tick(sender As Object, e As EventArgs) Handles Timer.Tick

        'Goes up by one each second that passess to update time of game
        IntTime += 1

    End Sub

    Private Sub GhostActivationTimer_Tick(sender As Object, e As EventArgs) Handles GhostActivationTimer.Tick

        GhostActivationCount += 1

        If GhostActivationCount = 70 Then '70 intervals before Ghost is active 
            PinkGhost.IsActive = True
            PinkGhost.changing = False

            'This allows the ghost to exit the ghost box right after they are activated
            Dim TempQueue As New Queue(Of Tile)
            TempQueue.Enqueue(New Tile(14, 12))
            TempQueue.Enqueue(New Tile(14, 13))
            TempQueue.Enqueue(New Tile(13, 13))
            TempQueue.Enqueue(New Tile(12, 13))
            TempQueue.Enqueue(New Tile(11, 13))

            PinkGhost.GhostPath = TempQueue
        End If

        If GhostActivationCount = 150 Then '150 intervals before Ghost is active 
            BlueGhost.IsActive = True
            BlueGhost.changing = False

            'This allows the ghost to exit the ghost box right after they are activated
            Dim TempQueue As New Queue(Of Tile)
            TempQueue.Enqueue(New Tile(14, 14))
            TempQueue.Enqueue(New Tile(14, 13))
            TempQueue.Enqueue(New Tile(13, 13))
            TempQueue.Enqueue(New Tile(12, 13))
            TempQueue.Enqueue(New Tile(11, 13))

            BlueGhost.GhostPath = TempQueue
        End If

        If GhostActivationCount = 270 Then '270 intervals before Ghost is active 
            OrangeGhost.IsActive = True
            OrangeGhost.changing = False

            'This allows the ghost to exit the ghost box right after they are activated
            Dim TempQueue As New Queue(Of Tile)
            TempQueue.Enqueue(New Tile(14, 13))
            TempQueue.Enqueue(New Tile(13, 13))
            TempQueue.Enqueue(New Tile(12, 13))
            TempQueue.Enqueue(New Tile(11, 13))

            OrangeGhost.GhostPath = TempQueue

            OrangeGhost.JustActivated = True
            GhostActivationTimer.Enabled = False
        End If

    End Sub

#End Region

#Region "User Key Presses and Movement Updates for Pacman"
    Private Sub Form2_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown 'Movement for Pacman 

        'Exit to pause the game
        If Keys.Escape And Control.Enabled = False Then
            If PnlExit.Visible = True Then
                If PnlGameOptions.Visible = False Then
                    BoolIsRunning = True
                End If
                PnlExit.Visible = False
            Else
                BoolIsRunning = False
                PnlExit.Visible = True
            End If
        End If

        'Movement for Pacman is controlled using the arrow keys:
        'Up 38  
        'Down 40
        'Left 37
        'Right 39



        If Control.Enabled = True Then
            Select Case e.KeyCode


                Case Keys.Up 'Key value = 38 
                    If InvertedControles = False Then
                        Pacman.NextDirection = Keys.Up
                        If IsNodeEmpty(Pacman.IX, Pacman.IY - 1) Then
                            Pacman.Direction = Keys.Up
                        End If
                    Else
                        Pacman.NextDirection = Keys.Down
                        If IsNodeEmpty(Pacman.IX, Pacman.IY + 1) Then
                            Pacman.Direction = Keys.Down
                        End If
                    End If
                Case Keys.W
                    If InvertedControles = False Then
                        Pacman.NextDirection = Keys.Up
                        If IsNodeEmpty(Pacman.IX, Pacman.IY - 1) Then
                            Pacman.Direction = Keys.Up
                        End If
                    Else
                        Pacman.NextDirection = Keys.Down
                        If IsNodeEmpty(Pacman.IX, Pacman.IY + 1) Then
                            Pacman.Direction = Keys.Down
                        End If
                    End If


                Case Keys.Down 'Key value = 40
                    If InvertedControles = False Then
                        Pacman.NextDirection = Keys.Down
                        If IsNodeEmpty(Pacman.IX, Pacman.IY + 1) Then
                            Pacman.Direction = Keys.Down
                        End If
                    Else
                        Pacman.NextDirection = Keys.Up
                        If IsNodeEmpty(Pacman.IX, Pacman.IY - 1) Then
                            Pacman.Direction = Keys.Up
                        End If
                    End If
                Case Keys.S
                    If InvertedControles = False Then
                        Pacman.NextDirection = Keys.Down
                        If IsNodeEmpty(Pacman.IX, Pacman.IY + 1) Then
                            Pacman.Direction = Keys.Down
                        End If
                    Else
                        Pacman.NextDirection = Keys.Up
                        If IsNodeEmpty(Pacman.IX, Pacman.IY - 1) Then
                            Pacman.Direction = Keys.Up
                        End If
                    End If


                Case Keys.Left 'Key value = 37
                    If InvertedControles = False Then
                        Pacman.NextDirection = Keys.Left
                        If IsNodeEmpty(Pacman.IX - 1, Pacman.IY) Then
                            Pacman.Direction = Keys.Left
                        End If
                    Else
                        Pacman.NextDirection = Keys.Right
                        If IsNodeEmpty(Pacman.IX + 1, Pacman.IY) Then
                            Pacman.Direction = Keys.Right
                        End If
                    End If
                Case Keys.A
                    If InvertedControles = False Then
                        Pacman.NextDirection = Keys.Left
                        If IsNodeEmpty(Pacman.IX - 1, Pacman.IY) Then
                            Pacman.Direction = Keys.Left
                        End If
                    Else
                        Pacman.NextDirection = Keys.Right
                        If IsNodeEmpty(Pacman.IX + 1, Pacman.IY) Then
                            Pacman.Direction = Keys.Right
                        End If
                    End If


                Case Keys.Right 'Key value = 39
                    If InvertedControles = False Then
                        Pacman.NextDirection = Keys.Right
                        If IsNodeEmpty(Pacman.IX + 1, Pacman.IY) Then
                            Pacman.Direction = Keys.Right
                        End If
                    Else
                        Pacman.NextDirection = Keys.Left
                        If IsNodeEmpty(Pacman.IX - 1, Pacman.IY) Then
                            Pacman.Direction = Keys.Left
                        End If
                    End If
                Case Keys.D
                    If InvertedControles = False Then
                        Pacman.NextDirection = Keys.Right
                        If IsNodeEmpty(Pacman.IX + 1, Pacman.IY) Then
                            Pacman.Direction = Keys.Right
                        End If
                    Else
                        Pacman.NextDirection = Keys.Left
                        If IsNodeEmpty(Pacman.IX - 1, Pacman.IY) Then
                            Pacman.Direction = Keys.Left
                        End If
                    End If


                Case Keys.Escape
                    If PnlExit.Visible = True Then
                        If PnlGameOptions.Visible = False Then
                            BoolIsRunning = True
                        End If
                        PnlExit.Visible = False
                    Else
                        BoolIsRunning = False
                        PnlExit.Visible = True
                    End If
            End Select
        End If

    End Sub

    'Function to check if a node is empty
    Private Function IsNodeEmpty(x As Integer, y As Integer) As Boolean
        'Check node not wall
        If x >= 0 AndAlso x < Maps(MapNo).GetLength(1) AndAlso y >= 0 AndAlso y < Maps(MapNo).GetLength(0) Then
            If Maps(MapNo)(y, x) < 2 Then
                Return True 'Node empty
            End If
        End If
        Return False 'Node not empty
    End Function

#End Region

#Region "Selection For UI"
    Private Sub TxtMap1_Click(sender As Object, e As EventArgs) Handles TxtMap1.Click

        SwitchSelectVisability(LblSelectedMap2, LblSelectedMap1)
        MapNo = 0

    End Sub

    Private Sub TxtMap2_Click(sender As Object, e As EventArgs) Handles TxtMap2.Click

        SwitchSelectVisability(LblSelectedMap1, LblSelectedMap2)
        MapNo = 1

    End Sub

    Private Sub LblTimed_Click(sender As Object, e As EventArgs) Handles LblTimed.Click

        SwitchSelectVisability(LblSelectedUntimed, LblSelectedTimed)
        IsTimedlevel = True

    End Sub
    Private Sub BtnTimed_Click(sender As Object, e As EventArgs) Handles BtnTimed.Click

        SwitchSelectVisability(LblSelectedUntimed, LblSelectedTimed)
        IsTimedlevel = True

    End Sub
    Private Sub LblUntimed_Click(sender As Object, e As EventArgs) Handles LblUntimed.Click

        SwitchSelectVisability(LblSelectedTimed, LblSelectedUntimed)
        IsTimedlevel = False

    End Sub
    Private Sub BtnUntimed_Click(sender As Object, e As EventArgs) Handles BtnUntimed.Click

        SwitchSelectVisability(LblSelectedTimed, LblSelectedUntimed)
        IsTimedlevel = False

    End Sub

    Private Sub SwitchSelectVisability(Label1 As Label, Label2 As Label)

        Label1.Visible = False
        Label2.Visible = True

    End Sub


#Region "Lable On/Off Switching"



    Private Sub OnOffLableChange(Label As Label)
        If Label.Text = "Off" Then
            Label.Text = "On"
        ElseIf Label.Text = "On" Then
            Label.Text = "Off"
        End If
    End Sub
    Private Sub lblPaintTrailsONOFF_Click(sender As Object, e As EventArgs) Handles lblPaintTrailsONOFF.Click
        OnOffLableChange(lblPaintTrailsONOFF)
    End Sub
    Private Sub lblPaintTrails_Click(sender As Object, e As EventArgs) Handles lblPaintTrails.Click
        OnOffLableChange(lblPaintTrailsONOFF)
    End Sub

    Private Sub lblSpeedX10_Click(sender As Object, e As EventArgs) Handles lblSpeedX10.Click
        OnOffLableChange(lblSpeedX10ONOFF)
    End Sub

    Private Sub lblSpeedX10ONOFF_Click(sender As Object, e As EventArgs) Handles lblSpeedX10ONOFF.Click
        OnOffLableChange(lblSpeedX10ONOFF)
    End Sub
    Private Sub lblLightsOut_Click(sender As Object, e As EventArgs) Handles lblLightsOut.Click
        OnOffLableChange(lblLightsOutONOFF)
    End Sub

    Private Sub lblLightsOutONOFF_Click(sender As Object, e As EventArgs) Handles lblLightsOutONOFF.Click
        OnOffLableChange(lblLightsOutONOFF)
    End Sub

    Private Sub lblBlackWhiteONOFF_Click(sender As Object, e As EventArgs) Handles lblBlackWhiteONOFF.Click
        OnOffLableChange(lblBlackWhiteONOFF)
    End Sub

    Private Sub lblBlackWhite_Click(sender As Object, e As EventArgs) Handles lblBlackWhite.Click
        OnOffLableChange(lblBlackWhiteONOFF)
    End Sub
    Private Sub lblInverted_Click(sender As Object, e As EventArgs) Handles lblInverted.Click
        OnOffLableChange(lblInvertedONOFF)
    End Sub

    Private Sub lblInvertedONOFF_Click(sender As Object, e As EventArgs) Handles lblInvertedONOFF.Click
        OnOffLableChange(lblInvertedONOFF)
    End Sub

    Private Sub lblDisco_Click(sender As Object, e As EventArgs) Handles lblDisco.Click
        OnOffLableChange(lblDiscoONOFF)
        If lblDiscoONOFF.Text = "On" Then
            MsgBox("         !!!Flashing Lights Warning!!!
This mode causes Fast Flashing Colours
              !!!Epilepsy Warning!!!")
        End If
    End Sub

    Private Sub lblDiscoONOFF_Click(sender As Object, e As EventArgs) Handles lblDiscoONOFF.Click
        OnOffLableChange(lblDiscoONOFF)
        If lblDiscoONOFF.Text = "On" Then
            MsgBox("         !!!Flashing Lights Warning!!!
This mode causes Fast Flashing Colours
              !!!Epilepsy Warning!!!")
        End If
    End Sub






#End Region

#End Region

End Class

'Object Oreinted
Public Class Entity

    Public IX As Integer
    Public IY As Integer
    Public BackTileX As Integer
    Public BackTileY As Integer
    Public IsActive As Boolean
    Public JustActivated As Boolean = False

    Protected MapNo As Integer = Form2.MapNo

    Protected verticalportaltravel As Boolean = False
    Protected horizontalportaltravel As Boolean = False


    'Stores all entitys directions
    Public Direction As Keys
    Public BackDirection As Keys
    Public NextDirection As Keys

    Protected Sub New(startX As Integer, startY As Integer)
        IX = startX
        IY = startY
    End Sub

    'Collision detector
    Public Overridable Sub Move(map(,) As Integer)

        BackTileX = IX
        BackTileY = IY

        Dim newX = IX
        Dim newY = IY

        Dim nextX = IX
        Dim nextY = IY

        Dim NextXYIsValid As Boolean = False

        Select Case NextDirection

            Case Keys.Up '38
                nextY -= 1
            Case Keys.W '38
                nextY -= 1

            Case Keys.Down '40
                nextY += 1
            Case Keys.S '40
                nextY += 1

            Case Keys.Left '37
                nextX -= 1
            Case Keys.A '37
                nextX -= 1

            Case Keys.Right '39
                nextX += 1
            Case Keys.D '39
                nextX += 1

        End Select

        If map(nextY, nextX) < 2 Then
            NextXYIsValid = True
            verticalportaltravel = False
            horizontalportaltravel = False
        End If

        If NextXYIsValid = True Then
            newX = nextX
            newY = nextY
            Direction = NextDirection

        Else

            'Direction property determines movement
            Select Case Direction
                Case Keys.Up '38
                    newY -= 1
                Case Keys.Down '40
                    newY += 1
                Case Keys.Left '37
                    newX -= 1
                Case Keys.Right '39
                    newX += 1
            End Select

        End If

        'Collision

        'Checks walls
        If map(newY, newX) < 2 Or map(newY, newX) = 3 Then
            IX = newX
            IY = newY
        End If

        'Checks portal 
        If IX = 0 Then
            IX = 26
            verticalportaltravel = True
        ElseIf IX = 27 Then
            IX = 1
            verticalportaltravel = True
        ElseIf IY = 0 Then
            IY = 29
            horizontalportaltravel = True
        ElseIf IY = 30 Then
            IY = 1
            horizontalportaltravel = True
        End If

    End Sub


    'GXF for Pacman
    Public Overridable Sub Draw(g As Graphics, tileSize As Integer)
        Dim entityRect As New Rectangle(IX * tileSize, IY * tileSize, tileSize, tileSize)
        g.FillEllipse(Brushes.Gray, entityRect)
    End Sub

    Public Overridable Sub DrawOverPreviousLocation(g As Graphics, tileSize As Integer, MapDisplay(,) As Integer, Paint As Boolean, Rest As Boolean)
        If Not BackTileX = 0 Or Not BackTileY = 0 Then
            Dim entityRect As New Rectangle(BackTileX * tileSize, BackTileY * tileSize, tileSize, tileSize)

            g.FillRectangle(Brushes.Black, entityRect)
            g.DrawRectangle(Pens.Black, entityRect)
        End If
    End Sub

End Class

Public Class Pacman
    Inherits Entity

    'Score
    Public Score As Integer
    Private TilesNotEmpty As Integer = 0
    Public NoPelletsLeft As Boolean = False
    Public MegaPelletEaten As Boolean = False
    Public GameSpeedIncreaseDueToPelletCount As Boolean = False
    Private NeedToUpdateVerticalPortals As Boolean = False
    Private NeedToUpdateHorizontalPortals As Boolean = False

    Private Mcount As Integer = 0 'Movemnet count 
    Public Mspeed As Integer = 5 'Movement speed

    Public Sub New(startX As Integer, startY As Integer)
        MyBase.New(startX, startY)
        Score = 0
    End Sub

#Region "Eats pellets/Portal Travel"
    Public Overrides Sub Move(map(,) As Integer)

        Mcount += 1

        If Mspeed <= Mcount Then

            MyBase.Move(map)
            Mcount = 0

        End If

        If verticalportaltravel = True Then
            If map(IY, IX - 1) = -1 Or map(IY, IX + 1) = -1 Then
                verticalportaltravel = False
                map(IY, 0) = -2
                map(IY, 27) = -2

                Score += 20
                TilesNotEmpty = 0
                NeedToUpdateVerticalPortals = True

                For icountx = 0 To 27 Step +1
                    For icounty = 0 To 30 Step +1
                        If map(icounty, icountx) = 1 Or map(icounty, icountx) = -1 Then
                            TilesNotEmpty += 1
                        End If
                    Next
                Next
            End If
        ElseIf horizontalportaltravel = True Then
            If map(IY + 1, IX) = -1 Or map(IY - 1, IX) = -1 Then
                horizontalportaltravel = False
                map(30, IX) = -2
                map(0, IX) = -2

                Score += 20

                NeedToUpdateHorizontalPortals = True
                TilesNotEmpty = 0

                For icountx = 0 To 27 Step +1
                    For icounty = 0 To 30 Step +1
                        If map(icounty, icountx) = 1 Or map(icounty, icountx) = -1 Then
                            TilesNotEmpty += 1
                        End If
                    Next
                Next
            End If
        ElseIf map(IY, IX) = 1 Or map(IY, IX) = 3 Then
            If map(IY, IX) = 3 Then
                MegaPelletEaten = True
            End If
            map(IY, IX) = 0
            Score += 10

            TilesNotEmpty = 0

            For icountx = 0 To 27 Step +1
                For icounty = 0 To 30 Step +1
                    If map(icounty, icountx) = 1 Or map(icounty, icountx) = -1 Then
                        TilesNotEmpty += 1
                    End If
                Next
            Next

        End If



        If TilesNotEmpty = 0 Then

            NoPelletsLeft = True

            Direction = 0
            NextDirection = 0
            verticalportaltravel = False
            horizontalportaltravel = False
            For icountx = 0 To 27 Step +1
                For icounty = 0 To 30 Step +1
                    If map(icounty, icountx) = 0 Then
                        map(icounty, icountx) = 1
                    End If
                    If map(icounty, icountx) = -2 Then
                        map(icounty, icountx) = -1
                    End If
                Next
            Next

        ElseIf TilesNotEmpty > 100 Then
            GameSpeedIncreaseDueToPelletCount = True
        End If
    End Sub

#End Region

    'Render Pacman
    Public Overrides Sub Draw(g As Graphics, tileSize As Integer)
        Dim PacManRect As New Rectangle(IX * tileSize, IY * tileSize, tileSize, tileSize)
        g.FillEllipse(Form1.PlayerSelelectedColour, PacManRect)
    End Sub
    Public Overrides Sub DrawOverPreviousLocation(g As Graphics, tileSize As Integer, MapDisplay(,) As Integer, paint As Boolean, rest As Boolean)

        'Once Pacman moves, his previous location remains displayed until cleared
        'This ensures pacman’s previous location is redrawn with a normal tile

        If (Not BackTileX = 0 Or Not BackTileY = 0) And paint = False Then
            Dim entityRect As New Rectangle(BackTileX * tileSize, BackTileY * tileSize, tileSize, tileSize)

            g.FillRectangle(Brushes.Black, entityRect)
            g.DrawRectangle(Pens.Black, entityRect)
        End If

        If NeedToUpdateVerticalPortals = True Then

            NeedToUpdateVerticalPortals = False
            Dim PortalRect1 As New Rectangle(27 * tileSize, BackTileY * tileSize, tileSize, tileSize)

            g.FillRectangle(Brushes.Red, PortalRect1)
            g.DrawRectangle(Pens.Black, PortalRect1)

            Dim PortalRect2 As New Rectangle(0 * tileSize, BackTileY * tileSize, tileSize, tileSize)

            g.FillRectangle(Brushes.Red, PortalRect2)
            g.DrawRectangle(Pens.Black, PortalRect2)

        ElseIf NeedToUpdateHorizontalPortals = True Then

            NeedToUpdateHorizontalPortals = False
            Dim PortalRect1 As New Rectangle(BackTileX * tileSize, 0 * tileSize, tileSize, tileSize)

            g.FillRectangle(Brushes.Red, PortalRect1)
            g.DrawRectangle(Pens.Black, PortalRect1)

            Dim PortalRect2 As New Rectangle(BackTileX * tileSize, 30 * tileSize, tileSize, tileSize)

            g.FillRectangle(Brushes.Red, PortalRect2)
            g.DrawRectangle(Pens.Black, PortalRect2)


        End If
    End Sub

End Class

Public Class Ghost
    Inherits Entity

    Protected Sub New(startX As Integer, startY As Integer)
        MyBase.New(startX, startY)
    End Sub

    'Display path (for admin)
    Protected sttracking As String

    'Ghosts go to Pacman
    Public TrackingIX As Integer
    Public TrackingIY As Integer

    'Is the Ghost changing directions? (180 turn)
    Public changing As Boolean = False
    Public Notchanging As Boolean = False

    'Ghosts go to their corners 
    Public TrackingSQIX(3) As Integer
    Public TrackingSQIY(3) As Integer

    'All modes 
    Public Scatter As Boolean
    Public frightened As Boolean
    Public Eaten As Boolean

    'Saving location of the most Previous tile/node Ghost was on 
    Public PreviousTileX As Integer
    Public PreviousTileY As Integer
    Public PreviousNodeX As Integer
    Public PreviousNodeY As Integer

    'Allowing ghosts to go through portals
    Private Ghostportaltravel As Boolean = False
    Private Ghostonportal As Boolean = False

    'Controls the Ghost speed 
    Public GhostSpeedCount As Integer

    Public Property GhostPath As Queue(Of Tile)

#Region "Ghost Map declerations"

    Public Shared WeightedGhostMaps As New List(Of Integer(,))()
    Public Shared GhostMaps As New List(Of Integer(,))()


    '            0  1  2  3  4  5  6  7  8  9 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24 25 26 27
    Public Shared WeightedGhostMapA(,) As Integer = {
                {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},  '0
                {2, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 1, 1, 2},  '1
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '2
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '3
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '4
                {2, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 0, 2},  '5
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '6
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '7
                {2, 1, 1, 1, 1, 1, 0, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 0, 1, 1, 1, 1, 1, 2},  '8
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '9
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '10
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '11
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '12
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '13
                {-1, 0, 1, 1, 1, 1, 0, 1, 1, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 1, 1, 0, 1, 1, 1, 1, 0, -1},  '14
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '15
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '16
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '17
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '18
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '19
                {2, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 2, 2, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 2},  '20
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '21
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '22
                {2, 1, 1, 1, 2, 2, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 2, 2, 1, 1, 1, 2},  '23
                {2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2},  '24
                {2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2},  '25
                {2, 1, 1, 0, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 0, 1, 1, 2},  '26
                {2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2},  '27
                {2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2},  '28
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '29
                {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2}   '30
            }

    Public Shared GhostMapA(,) As Integer = {
                {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},  '0
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '1
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '2
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '3
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '4
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '5
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '6
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '7
                {2, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 2},  '8
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '9
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '10
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '11
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '12
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '13
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '14
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '15
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '16
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '17
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '18
                {2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2},  '19
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '20
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '21
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '22
                {2, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 2},  '23
                {2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2},  '24
                {2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2},  '25
                {2, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 2, 2, 1, 1, 1, 1, 1, 1, 2},  '26
                {2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2},  '27
                {2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2},  '28
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '29
                {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2}   '30
            }

    Public Shared WeightedGhostMapB(,) As Integer = {
                {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1, 2, 2, -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},  '0
                {2, 1, 1, 1, 1, 1, 0, 1, 1, 1, 2, 2, 0, 2, 2, 0, 2, 2, 1, 1, 1, 0, 1, 1, 1, 1, 1, 2},  '1
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '2
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '3
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '4
                {2, 0, 1, 1, 1, 1, 0, 0, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 0, 0, 1, 1, 1, 1, 0, 2},  '5
                {2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2},  '6
                {2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2},  '7
                {2, 1, 2, 2, 1, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 1, 0, 1, 1, 1, 2, 2, 1, 2},  '8
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '9
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '10
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '11
                {2, 1, 1, 1, 0, 1, 1, 1, 1, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 1, 1, 1, 1, 0, 1, 1, 1, 2},  '12
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '13
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '14
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '15
                {-1, 0, 1, 1, 0, 1, 1, 1, 1, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 1, 1, 1, 1, 0, 1, 1, 0, -1},'16
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '17
                {2, 1, 1, 1, 0, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 0, 1, 1, 1, 2},  '18
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '19
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '20
                {2, 1, 2, 2, 0, 1, 1, 1, 1, 0, 2, 2, 2, 2, 2, 2, 2, 2, 0, 1, 1, 1, 1, 0, 2, 2, 1, 2},  '21
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '22
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 0, 1, 1, 0, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '23
                {2, 0, 1, 1, 0, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 0, 1, 1, 0, 2},  '24
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '25
                {2, 1, 2, 2, 1, 0, 1, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 0, 1, 1, 1, 0, 1, 2, 2, 1, 2},  '26
                {2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2},  '27
                {2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2},  '28
                {2, 1, 1, 1, 1, 0, 1, 1, 1, 1, 2, 2, 0, 2, 2, 0, 2, 2, 1, 1, 1, 1, 0, 1, 1, 1, 1, 2},  '29
                {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, -1, 2, 2, -1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2} '30
            }

    Public Shared GhostMapB(,) As Integer = {
                {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2},  '0
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '1
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '2
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '3
                {2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2},  '4
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '5
                {2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2},  '6
                {2, 1, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 1, 2},  '7
                {2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2},  '8
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '9
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '10
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '11
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '12
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '13
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '14
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '15
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '16
                {2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2},  '17
                {2, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 2},  '18
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '19
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '20
                {2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2},  '21
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '22
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '23
                {2, 1, 1, 1, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 1, 1, 1, 2},  '24
                {2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2, 2, 2, 2, 2, 2, 2, 1, 2, 2, 1, 2},  '25
                {2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2},  '26
                {2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2},  '27
                {2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 2, 2, 2, 1, 2, 2, 2, 1, 2},  '28
                {2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 1, 2, 2, 1, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2},  '29
                {2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2}   '30
            }

#End Region

#Region "Pathfinding Algorithms and Associated Code"
    Public Function FindPathDijkstras(startX As Integer, startY As Integer, endX As Integer, endY As Integer) As List(Of Node)

        'Checks portal and sides
        If startX = 0 Or endX = 0 Then
            Return Nothing
        End If


        If endX = PreviousNodeX And endY = PreviousNodeY Then 'Copied triple from BFS
            Dim directions As New List(Of Tuple(Of Integer, Integer)) From {
    Tuple.Create(0, 1),    ' Right
    Tuple.Create(1, 0),    ' Down
    Tuple.Create(0, -1),   ' Left
    Tuple.Create(-1, 0)    ' Up
}


            'Finding next direction to go in to find next node 
            For Each Ddirection In directions
                Dim newX = endX + Ddirection.Item1
                Dim newY = endY + Ddirection.Item2
                If GhostMaps(MapNo)(newX, newY) = 1 And newX <> endX And newY <> endY Then
                    endX = newX
                    endY = newY
                End If
            Next
        End If

        Dim openList As New List(Of Node)() 'List for Dijkstras
        Dim closedList As New List(Of String)()
        Dim startNode As New Node(startY, startX, 0)
        startNode.G = 0 'Start node has G cost of 0
        Dim endNode As New Node(endX, endY, 0)

        openList.Add(startNode)

        'Checks if nodes need to be created and creats a node at the points need be
        Dim Nodeatstart As Boolean = True
        Dim Nodeatend As Boolean = True

        If WeightedGhostMaps(MapNo)(startY, startX) <> 0 Then
            WeightedGhostMaps(MapNo)(startY, startX) = 0
            Nodeatstart = False
        End If
        If WeightedGhostMaps(MapNo)(endX, endY) <> 0 Then
            WeightedGhostMaps(MapNo)(endX, endY) = 0
            Nodeatend = False
        End If


        'Actual Dijkstras pathfinding algorithm
        While openList.Count > 0

            'Get node with lowest G cost
            openList.Sort(Function(nodeA, nodeB) nodeA.G.CompareTo(nodeB.G))
            Dim currentNode = openList(0)

            'If reached end reconstruct path
            If currentNode.X = endNode.X AndAlso currentNode.Y = endNode.Y Then

                If Nodeatstart = False Then
                    WeightedGhostMaps(MapNo)(startY, startX) = 1
                End If
                If Nodeatend = False Then
                    WeightedGhostMaps(MapNo)(endX, endY) = 1
                End If

                Return ReconstructPathDijkstras(currentNode)

            End If

            openList.Remove(currentNode)
            closedList.Add($"{currentNode.X},{currentNode.Y}")

            'Check neighbors
            For Each neighbor In DijkstrasGetNeighbors(currentNode) 'Gets neighbors for current node (wall or next node)


                If Ghostportaltravel = True Then
                    Ghostportaltravel = False
                    openList.Add(currentNode)
                    Return openList
                End If

                Dim neighborKey = $"{neighbor.X},{neighbor.Y}"

                'Use list.contains for existence check
                If closedList.Contains(neighborKey) OrElse WeightedGhostMaps(MapNo)(neighbor.X, neighbor.Y) > 1 OrElse WeightedGhostMaps(MapNo)(neighbor.X, neighbor.Y) < -1 Then
                    Continue For
                End If

                'Weight added to current G cost 
                Dim tentativeG = currentNode.G + neighbor.W

                'Check if neighbor has lower G cost and in openList
                If Not openList.Contains(neighbor) OrElse tentativeG < neighbor.G Then
                    neighbor.G = tentativeG
                    neighbor.Parent = currentNode

                    If Not openList.Contains(neighbor) Then
                        openList.Add(neighbor)
                    End If
                End If
            Next
        End While

        'Reverts nodes at the start and end if changed
        If Nodeatstart = False Then
            WeightedGhostMaps(MapNo)(startX, startY) = 1
        End If
        If Nodeatend = False Then
            WeightedGhostMaps(MapNo)(endX, endY) = 1
        End If
        If Nodeatstart = False Then
            WeightedGhostMaps(MapNo)(startY, startX) = 1
        End If
        If Nodeatend = False Then
            WeightedGhostMaps(MapNo)(endX, endY) = 1
        End If

        Return Nothing 'No path found
    End Function
    Private Function DijkstrasGetNeighbors(node As Node) As List(Of Node) 'ChatGPT introdused using list of tuples and showed how to use it 

        Dim neighbors As New List(Of Node)
        Dim directions As New List(Of Tuple(Of Integer, Integer)) From {
                Tuple.Create(0, 1),    ' Right
                Tuple.Create(1, 0),    ' Down
                Tuple.Create(0, -1),   ' Left
                Tuple.Create(-1, 0)    ' Up
            }

        For Each Adirection In directions

            'Representing the weight 
            Dim TotalWeight As Integer = 1

            Dim newIX = node.X + Adirection.Item1
            Dim newIY = node.Y + Adirection.Item2

            'To see paths to go to find neighbors and sees walls 
            If newIX >= 0 AndAlso newIX < WeightedGhostMaps(MapNo).GetLength(0) AndAlso newIY >= 0 AndAlso newIY < WeightedGhostMaps(MapNo).GetLength(1) Then
                If WeightedGhostMaps(MapNo)(newIX, newIY) = 2 Then
                    TotalWeight = 9999
                    neighbors.Add(New Node(newIX, newIY, TotalWeight))
                ElseIf WeightedGhostMaps(MapNo)(newIX, newIY) = -1 Then
                    TotalWeight = 9999
                    neighbors.Add(New Node(newIX, newIY, TotalWeight))
                ElseIf newIX = PreviousTileY And newIY = PreviousTileX Then
                    TotalWeight = 9999
                    neighbors.Add(New Node(newIX, newIY, TotalWeight))
                    Continue For
                ElseIf WeightedGhostMaps(MapNo)(newIX, newIY) = 0 Then
                    neighbors.Add(New Node(newIX, newIY, TotalWeight))
                End If

            End If

            If newIY >= 28 Or newIY < 0 Or newIX >= 31 Or newIX < 0 Then
                Ghostportaltravel = True
                neighbors.Add(New Node(newIX, newIY, TotalWeight))
                Exit For
            End If

            'Finds neighbor node 
            If WeightedGhostMaps(MapNo)(newIX, newIY) = 1 Then

                'Simple starting loop to quickly find out how long path is in one direction 
                Do Until WeightedGhostMaps(MapNo)(newIX, newIY) = 0 Or WeightedGhostMaps(MapNo)(newIX, newIY) = 2 'Seeing if it hits an intresection (0) or a wall (2)

                    'Each loop add 1 to weight 
                    TotalWeight += 1

                    'Weight multiplyed to direction so keep track of how many spaces it has moved in the respected direction
                    newIX = (node.X + Adirection.Item1 * TotalWeight)
                    newIY = (node.Y + Adirection.Item2 * TotalWeight)

                Loop

                'After this point one of two thing can happen 

                'Number 1 (The solution is quick and easy) :)
                'The path leeds to a intersection where the node at the intresection is recognised as the neighbor node (e.g when map (x,y) = 0, node is found)

                If WeightedGhostMaps(MapNo)(newIX, newIY) = 0 Then

                    neighbors.Add(New Node(newIX, newIY, TotalWeight))

                    'Number 2 (The solution is harder to get to) :(
                    'The path leeds to a wall which imples that it has turned if that happens the next lines will:

                    '- Find the direction of the continued path 
                    '- Go in that direction until it hits another wall (2) or intersection (0)

                ElseIf WeightedGhostMaps(MapNo)(newIX, newIY) = 2 Then

                    'Stores path it came from
                    'OldDir means the old Direction it came from to get the the current straight path
                    'This value is being stored to make sure that the path being found doesnt check the path it came from as this causes the 

                    Dim oldDirIX = (node.X + Adirection.Item1 * (TotalWeight - 2))
                    Dim oldDirIY = (node.Y + Adirection.Item2 * (TotalWeight - 2))

                    'As the waight was going from the node to the wall we need to subtract 1 to keep the waight on the path 
                    TotalWeight -= 1

                    'Stores the turn it took 
                    Dim oldIX = (node.X + Adirection.Item1 * TotalWeight)
                    Dim oldIY = (node.Y + Adirection.Item2 * TotalWeight)

                    'Does untill it finds a node (the neighbor)
                    'This is the point the algorithm will return to when a new turn is detected in the path 
                    'This unitl loop is forsed to stop after it finds the neighbor 
                    Do Until WeightedGhostMaps(MapNo)(newIX, newIY) = 0

                        'New weight for new straight path 
                        Dim NewPathWeight As Integer = 1
                        Dim NextDirectionX As Integer
                        Dim NextDirectionY As Integer

                        'Finding the new direction for the newly found path
                        For Each Bdirection In directions

                            newIX = oldIX + Bdirection.Item1
                            newIY = oldIY + Bdirection.Item2

                            If ((WeightedGhostMaps(MapNo)(newIX, newIY) = 0) Or (WeightedGhostMaps(MapNo)(newIX, newIY) = 1) Or (WeightedGhostMaps(MapNo)(newIX, newIY) = -1)) And (oldDirIX <> newIX Or oldDirIY <> newIY) Then
                                NextDirectionX = Bdirection.Item1
                                NextDirectionY = Bdirection.Item2
                                Exit For
                            End If
                        Next

                        If WeightedGhostMaps(MapNo)(newIX, newIY) = 0 Then 'Checks if valid neighbor (neighbor value = 0)

                            TotalWeight += NewPathWeight
                            neighbors.Add(New Node(newIX, newIY, TotalWeight))

                        ElseIf WeightedGhostMaps(MapNo)(newIX, newIY) = 1 And (oldDirIX <> newIX Or oldDirIY <> newIY) Then 'Seeing if new path isnt old path 

                            'Finds weight of the new straight path 
                            Do Until WeightedGhostMaps(MapNo)(newIX, newIY) = 0 Or WeightedGhostMaps(MapNo)(newIX, newIY) = 2

                                'Adds 1 to the new path weight
                                NewPathWeight += 1

                                'Applying new weight (length) to path to find node or wall
                                newIX = (oldIX + NextDirectionX * NewPathWeight)
                                newIY = (oldIY + NextDirectionY * NewPathWeight)

                                If WeightedGhostMaps(MapNo)(newIX, newIY) = 2 Then 'If it finds a wall then it repeats the 

                                    'If the algorithm gets here it means that the path has multible terns

                                    'At this point the algorithm:

                                    '- Adds the path weight if calculated to the total waight 
                                    '- The new turn replaces the old direction (this is so the algorithm can calculate the next weight up untill it hits the next neigbor (0) or wall (2))
                                    '- This is repeated untill it finds a neighbor (e.g when map (x,y) = 0 )

                                    'Stores path it came from 
                                    oldDirIX = (oldIX + NextDirectionX * (NewPathWeight - 2))
                                    oldDirIY = (oldIY + NextDirectionY * (NewPathWeight - 2))

                                    'Weight leads to wall, one is subtracted to keep weight on path 
                                    NewPathWeight -= 1

                                    oldIX += NextDirectionX * NewPathWeight 'stores turn it came from 
                                    oldIY += NextDirectionY * NewPathWeight

                                    'Adds the new path weight to the old path weight
                                    TotalWeight = NewPathWeight + TotalWeight


                                ElseIf WeightedGhostMaps(MapNo)(newIX, newIY) = 0 Then 'Neigbor is finaly found job done 

                                    TotalWeight += NewPathWeight
                                    neighbors.Add(New Node(newIX, newIY, TotalWeight))

                                End If

                            Loop 'Loop for finding weight of newly found path 

                        End If

                    Loop 'Loop until it has found a valid negibor

                End If
            End If

        Next 'Once at this point the next direction will be prossesed 

        Return neighbors
    End Function
    Private Function ReconstructPathDijkstras(endNode As Node) As List(Of Node)

        'Stores calculated path in list 
        Dim path As New List(Of Node)
        Dim currentNode = endNode

        'Adds every node to list 
        While currentNode IsNot Nothing
            path.Add(currentNode)
            currentNode = currentNode.Parent
        End While

        'Corrects the order
        path.Reverse()
        Return path
    End Function
    Private Function FindPathBFS(startX As Integer, startY As Integer, endX As Integer, endY As Integer) As Queue(Of Tile) 'Breadth first serach for paths between nodes 

        'Queue for BFS (following FIFO)
        Dim openqueue As New Queue(Of Tile)()
        Dim closedList As New List(Of String)()

        Dim starttile As New Tile(startY, startX)
        Dim endtile As New Tile(endX, endY)

        openqueue.Enqueue(starttile)

        'Actual BFS pathfinding algorithm
        While openqueue.Count > 0

            'Selects first value in queue 
            Dim currenttile = openqueue(0)

            'If reached end reconstruct path
            If currenttile.X = endtile.X AndAlso currenttile.Y = endtile.Y Then

                Return ReconstructPathBFS(currenttile)

            End If

            'Dequeue first value 
            openqueue.Dequeue()
            closedList.Add($"{currenttile.X},{currenttile.Y}")

            'Check neighbors for first value 
            For Each neighbor In GetNeighborsBFS(currenttile) 'Gets the neighbors for current tile (next wall or next tile)
                Dim neighborKey = $"{neighbor.X},{neighbor.Y}"

                'Use list.contains for existence checks (sees if neighbor is already in openqueue)
                If closedList.Contains(neighborKey) OrElse WeightedGhostMaps(MapNo)(neighbor.X, neighbor.Y) > 1 OrElse WeightedGhostMaps(MapNo)(neighbor.X, neighbor.Y) < -1 Then
                    Continue For
                End If

                'Equeues the tile to openqueue if not in openqueue
                If Not openqueue.Contains(neighbor) Then
                    neighbor.Parent = currenttile

                    If Not openqueue.Contains(neighbor) Then
                        openqueue.Enqueue(neighbor)
                    End If
                End If
            Next 'Repeat untill the first value in queue match end cords

        End While

        'No path found 
        Return Nothing
    End Function
    Private Function GetNeighborsBFS(tile As Tile) As List(Of Tile) 'ChatGPT showed me this method (from this new found knowlage of tuple i used it in Dijkstras)

        Dim neighbors As New List(Of Tile)
        Dim directions As New List(Of Tuple(Of Integer, Integer)) From {
            Tuple.Create(0, 1),    ' Right
            Tuple.Create(1, 0),    ' Down
            Tuple.Create(0, -1),   ' Left
            Tuple.Create(-1, 0)    ' Up
        }

        For Each Ddirection In directions
            Dim newIX = tile.X + Ddirection.Item1
            Dim newIY = tile.Y + Ddirection.Item2
            If (newIX >= 0 AndAlso newIX < WeightedGhostMaps(MapNo).GetLength(0) AndAlso newIY >= 0 AndAlso newIY < WeightedGhostMaps(MapNo).GetLength(1)) And Not (newIX = PreviousTileY And newIY = PreviousTileX And changing = False) Then
                neighbors.Add(New Tile(newIX, newIY))
            End If
        Next

        Return neighbors
    End Function 'End of chat GPT advice 
    Private Function ReconstructPathBFS(endtile As Tile) As Queue(Of Tile)

        'Stores calculation in a queue
        Dim queue As New Queue(Of Tile)
        Dim path As New List(Of Tile) 'For reversing the queue 
        Dim currenttile = endtile

        'Adds every tile to list 
        While currenttile IsNot Nothing
            path.Add(currenttile)
            currenttile = currenttile.Parent
        End While

        'Reverses the list
        path.Reverse()

        'Enqueue every tile in list 
        For Each tile In path
            queue.Enqueue(tile)
        Next

        Return queue
    End Function

#End Region

#Region "Ghost Modes and All other Ghost Subs and Functions"
    Protected Function PortalChecksFunction()

        'Checks for any protal collisions 

        'If there is a portal collision 
        'Enlist the appropriat next tiles 
        'Return true so that the other code dosnt get run unnecessarily

        'If no portals are detected 
        'Return false so sub can continue 

        If Not WeightedGhostMaps(MapNo)(PreviousTileY, PreviousTileX) = -1 And WeightedGhostMaps(MapNo)(IY, IX + 1) = -1 Then 'Portals on Right side of the screen

            Dim TempQueue As New Queue(Of Tile)
            TempQueue.Enqueue(New Tile(IY, 26))
            TempQueue.Enqueue(New Tile(IY, 27))
            TempQueue.Enqueue(New Tile(IY, 0))
            TempQueue.Enqueue(New Tile(IY, 1))

            GhostPath = TempQueue

            Return True

        ElseIf Not WeightedGhostMaps(MapNo)(PreviousTileY, PreviousTileX) = -1 And WeightedGhostMaps(MapNo)(IY, IX - 1) = -1 Then 'Portals on Left side of the screen

            Dim TempQueue As New Queue(Of Tile)
            TempQueue.Enqueue(New Tile(IY, 1))
            TempQueue.Enqueue(New Tile(IY, 0))
            TempQueue.Enqueue(New Tile(IY, 27))
            TempQueue.Enqueue(New Tile(IY, 26))

            GhostPath = TempQueue

            Return True

        ElseIf Not WeightedGhostMaps(MapNo)(PreviousTileY, PreviousTileX) = -1 And WeightedGhostMaps(MapNo)(IY - 1, IX) = -1 Then 'Portals on the top of the screen

            Dim TempQueue As New Queue(Of Tile)

            TempQueue.Enqueue(New Tile(1, IX))
            TempQueue.Enqueue(New Tile(0, IX))
            TempQueue.Enqueue(New Tile(30, IX))
            TempQueue.Enqueue(New Tile(29, IX))
            TempQueue.Enqueue(New Tile(28, IX))

            GhostPath = TempQueue

            Return True

        ElseIf Not WeightedGhostMaps(MapNo)(PreviousTileY, PreviousTileX) = -1 And WeightedGhostMaps(MapNo)(IY + 1, IX) = -1 Then 'Portals on the bottom of the screen

            Dim TempQueue As New Queue(Of Tile)

            TempQueue.Enqueue(New Tile(29, IX))
            TempQueue.Enqueue(New Tile(30, IX))
            TempQueue.Enqueue(New Tile(0, IX))
            TempQueue.Enqueue(New Tile(1, IX))
            TempQueue.Enqueue(New Tile(2, IX))

            GhostPath = TempQueue

            Return True

        End If

        Return False

    End Function

    Protected Sub EatenUpdateProtocalSub()

        If IX = 13 And IY = 11 Then

            Dim TempQueue As New Queue(Of Tile)
            TempQueue.Enqueue(New Tile(11, 13))
            TempQueue.Enqueue(New Tile(12, 13))
            TempQueue.Enqueue(New Tile(13, 13))
            TempQueue.Enqueue(New Tile(14, 13))
            TempQueue.Enqueue(New Tile(13, 13))
            TempQueue.Enqueue(New Tile(12, 13))
            TempQueue.Enqueue(New Tile(11, 13))

            GhostPath = TempQueue

            Eaten = False

        Else
            TrackingIX = 13
            TrackingIY = 11

            frightened = False

        End If

    End Sub

    Protected Sub FrightednedUpdateProtocalSub()

        'If Current location is intersection between 3 or more direction 
        If WeightedGhostMaps(MapNo)(IY, IX) = 0 And Not GhostMaps(MapNo)(PreviousTileY, PreviousTileX) = -1 Then

            'Creats list to store valid directions
            Dim ValidDirection As New List(Of Tile)

            'Adds the directions that are not walls and not the direction Ghost came from 

            Dim directions As New List(Of Tuple(Of Integer, Integer)) From {
    Tuple.Create(0, 1),    ' Down
    Tuple.Create(1, 0),    ' Right
    Tuple.Create(0, -1),   ' Left
    Tuple.Create(-1, 0)    ' Up
}

            For Each Ddirection In directions
                Dim newIX = IX + Ddirection.Item1
                Dim newIY = IY + Ddirection.Item2
                If newIX >= 0 AndAlso newIY >= 0 Then
                    If Not (newIX = PreviousTileX And newIY = PreviousTileY) And GhostMaps(MapNo)(newIY, newIX) = 1 Then
                        ValidDirection.Add(New Tile(newIX, newIY))
                    End If
                End If
            Next

            'Generates random direction 
            Dim rand As New Random()
            Dim RandomDirection As Integer = rand.Next(0, ValidDirection.Count) 'Random direction = a number between 0 and how many values the list stores 
            PreviousTileX = IX
            PreviousTileY = IY
            IX = ValidDirection(RandomDirection).X
            IY = ValidDirection(RandomDirection).Y

            '100% Randomes with no pathfinding 
            GhostPath = Nothing

        ElseIf WeightedGhostMaps(MapNo)(IY, IX) = 1 Then 'If there is 1 direction 

            'Finds the direction of the one possibe direction 
            Dim directions As New List(Of Tuple(Of Integer, Integer)) From {
    Tuple.Create(0, 1),    ' Down
    Tuple.Create(1, 0),    ' Right
    Tuple.Create(0, -1),   ' Left
    Tuple.Create(-1, 0)    ' Up
}

            For Each Ddirection In directions
                Dim newIX = IX + Ddirection.Item1
                Dim newIY = IY + Ddirection.Item2
                If newIX >= 0 AndAlso newIY >= 0 Then
                    If Not (newIX = PreviousTileX And newIY = PreviousTileY) And GhostMaps(MapNo)(newIY, newIX) = 1 Then 'Direction cant be wall or Previous direction
                        PreviousTileX = IX
                        PreviousTileY = IY
                        IX = newIX
                        IY = newIY
                        Exit For
                    End If
                End If
            Next
        End If

    End Sub

    Protected Sub TopRightScatterSQProtocal()
        If changing = True Then
            changing = True
        ElseIf (IX = TrackingSQIX(0) And IY = TrackingSQIY(0)) Or (Direction = 37 And IY = 1) Then 'When tracking SQ is corner or when at the top going left Ghost will track next corner squar (this stops Ghost from not moving from corner tile)
            TrackingIX = TrackingSQIX(1)
            TrackingIY = TrackingSQIY(1)
        ElseIf IY = TrackingSQIY(1) And IX = TrackingSQIX(1) And Direction = 39 Then 'Prevents Ghost from going the wrong direction in corner by forcing down when going anticlockwise (when in lower right corner)
            TrackingIY = TrackingSQIY(1) + 1
            TrackingIX = TrackingSQIX(1)
        ElseIf IY = TrackingSQIY(2) And IX = TrackingSQIX(2) And Direction = 40 Then 'Prevents Ghost from going the wrong direction in corner by forcing left when going anticlockwise (when in lower left corner)
            TrackingIY = TrackingSQIY(2)
            TrackingIX = TrackingSQIX(2) - 1
        ElseIf IY = TrackingSQIY(3) And IX = TrackingSQIX(3) And Not Direction = 39 Then 'Gives Ghost place to go when going anti clockwise to make it clockwise 
            TrackingIY = TrackingSQIY(3) - 1
            TrackingIX = TrackingSQIX(3)
        Else 'Ghost goes to corner 
            TrackingIX = TrackingSQIX(0)
            TrackingIY = TrackingSQIY(0)
        End If
    End Sub

    Protected Sub TopLeftScatterSQProtocal()
        If changing = True Then
            changing = True
        ElseIf (IX = TrackingSQIX(0) And IY = TrackingSQIY(0)) Or ((Direction = 37 And IY = 1) And Not IX = TrackingSQIX(1)) Then 'When tracking SQ is corner or when at the top going left Ghost will track next corner squar (this stops Ghost from not moving from corner tile)
            TrackingIX = TrackingSQIX(1)
            TrackingIY = TrackingSQIY(1)
        ElseIf IY = TrackingSQIY(1) And IX = TrackingSQIX(1) And Direction = 37 Then 'Prevents Ghost from going the wrong direction in corner by forcing down when going anticlockwise (when in lower right corner)
            TrackingIY = TrackingSQIY(1) + 1
            TrackingIX = TrackingSQIX(1)
        ElseIf IY = TrackingSQIY(2) And IX = TrackingSQIX(2) And Direction = 40 Then 'Prevents Ghost from going the wrong direction in corner by forcing left when going anticlockwise (when in lower left corner)
            TrackingIY = TrackingSQIY(2)
            TrackingIX = TrackingSQIX(2) + 1
        ElseIf IY = TrackingSQIY(3) And IX = TrackingSQIX(3) And Not Direction = 37 Then 'Gives Ghost place to go when going anti clockwise to make it clockwise 
            TrackingIY = TrackingSQIY(3) - 1
            TrackingIX = TrackingSQIX(3)
        Else 'Ghost goes to corner 
            TrackingIX = TrackingSQIX(0)
            TrackingIY = TrackingSQIY(0)
        End If
    End Sub

    Protected Sub BottomRightScatterSQProtocal()
        If changing = True Then
            changing = True
        ElseIf (Direction = 38 Or Direction = 37) And IY < TrackingSQIY(1) Then
            TrackingIX = TrackingSQIX(1)
            TrackingIY = TrackingSQIY(1)
        ElseIf (Direction = 39 Or Direction = 40) And IX < TrackingSQIX(2) Then
            TrackingIX = TrackingSQIX(2)
            TrackingIY = TrackingSQIY(2)
        Else 'Ghost goes to corner 
            TrackingIX = TrackingSQIX(0)
            TrackingIY = TrackingSQIY(0)
        End If
    End Sub

    Protected Sub BottomLeftScatterSQProtocal()
        If changing = True Then
            changing = True
        ElseIf (Direction = 38 Or Direction = 39) And IY > TrackingSQIY(1) Then
            TrackingIX = TrackingSQIX(1)
            TrackingIY = TrackingSQIY(1)
        ElseIf (Direction = 37 Or Direction = 40) And IX > TrackingSQIX(2) Then
            TrackingIX = TrackingSQIX(2)
            TrackingIY = TrackingSQIY(2)
        Else 'Ghost goes to corner 
            TrackingIX = TrackingSQIX(0)
            TrackingIY = TrackingSQIY(0)
        End If
    End Sub

    Protected Sub GhostChaseProtocalSub()
        'Chase mode
        If GhostPath Is Nothing Then

            'Finds weighted path with Djikstras algorithm 
            Dim WeightedGhostPath As List(Of Node) = FindPathDijkstras(IX, IY, TrackingIY, TrackingIX)

            If WeightedGhostPath Is Nothing Then
                Exit Sub
            End If


            'Makes sure that the ghosts is tracking the correct square when going through the protals 
            'Only applys to left and up
            If Direction = 39 Then 'Up
                Do Until WeightedGhostPath.Count = 2
                    WeightedGhostPath.RemoveAt(0)
                Loop
            ElseIf Direction = 37 Then 'Left
                Do Until WeightedGhostPath.Count = 2
                    WeightedGhostPath.RemoveAt(0)
                Loop
            End If

            'Checks if path found if found 
            If (WeightedGhostPath IsNot Nothing AndAlso WeightedGhostPath.Count > 1) Then
                'Replaces Previous node with most Previous node
                If WeightedGhostMaps(MapNo)(WeightedGhostPath(0).X, WeightedGhostPath(0).Y) = 0 Then
                    PreviousNodeX = WeightedGhostPath(0).Y
                    PreviousNodeY = WeightedGhostPath(0).X
                End If
                'Tracks next node for BFS
                TrackingIX = WeightedGhostPath(1).X
                TrackingIY = WeightedGhostPath(1).Y

            End If

            'Finds path with BFS
            GhostPath = FindPathBFS(IX, IY, TrackingIX, TrackingIY)
            If GhostPath IsNot Nothing Then

                Updateloaction()

            End If

        ElseIf GhostPath.Count > 1 Then 'If more than one cords in ghostpath

            Updateloaction()

        ElseIf GhostPath.Count = 1 Then 'If only one pair of cords in ghostpath (this pair of cords in question is ghosts current location)

            'Finds weighted path with Djikstras algorithm 
            Dim WeightedGhostPath As List(Of Node) = FindPathDijkstras(IX, IY, TrackingIY, TrackingIX)

            'Checks if path found if found 
            If WeightedGhostPath IsNot Nothing AndAlso Not WeightedGhostPath.Count = 1 Then

                If WeightedGhostPath.Count > 1 Then
                    'Replaces Previous node with most Previous node
                    If WeightedGhostMaps(MapNo)(WeightedGhostPath(0).X, WeightedGhostPath(0).Y) = 0 Then
                        PreviousNodeX = WeightedGhostPath(0).Y
                        PreviousNodeY = WeightedGhostPath(0).X
                    End If
                    'Tracks next node for BFS
                    TrackingIX = WeightedGhostPath(1).X
                    TrackingIY = WeightedGhostPath(1).Y

                ElseIf WeightedGhostPath.Count = 1 Then 'Error happened here fix was swapping the cords around
                    Dim swapxy = TrackingIX
                    TrackingIX = IY
                    TrackingIY = swapxy
                End If

                'Finds path with BFS
                GhostPath = FindPathBFS(IX, IY, TrackingIX, TrackingIY)

                If GhostPath IsNot Nothing Then
                    If GhostPath.Count > 1 Then

                        Updateloaction()

                    End If
                End If
            ElseIf WeightedGhostPath IsNot Nothing AndAlso WeightedGhostPath.Count = 1 Then

                'If the taregt tile dose not change while the Ghost is one the target tile the Ghost is softlocked
                'tracks Previous node so teh Ghost is not softlocked

                TrackingIX = PreviousNodeX
                TrackingIY = PreviousNodeY

                'Runs Funtion again with new tracking variables
                GhostChaseProtocalSub()

            End If
        Else
            'Finds path with BFS
            If Not PreviousNodeY = 0 And Not PreviousNodeX = 0 Then
                GhostPath = FindPathBFS(IX, IY, PreviousNodeY, PreviousNodeX)

                If GhostPath IsNot Nothing Then
                    If GhostPath.Count > 1 Then

                        Updateloaction()

                        changing = False

                    End If
                Else

                    'If the Ghost has recently been activated and is changing it makes sure that 
                    Dim TempIX = PreviousTileX
                    Dim TenpIY = PreviousTileY

                    PreviousTileX = IX
                    PreviousTileY = IY

                    IX = TempIX
                    IY = TenpIY

                    changing = False

                End If
            End If
        End If

    End Sub

    Private Sub Updateloaction()
        'Replaces Previous tile with most Previous tile
        PreviousTileX = GhostPath(0).Y
        PreviousTileY = GhostPath(0).X

        GhostPath.Dequeue()

        'Moves Ghost to next location 
        IX = GhostPath(0).Y
        IY = GhostPath(0).X
    End Sub

    Protected Sub GhostChangingToOppositeDirectionSub()

        'Finds Previous path with BFS
        'The Ghost can not cahnge direction is x and x is nothing or y and x are the starting possition of the ghosts

        If Not PreviousNodeY = 0 And Not PreviousNodeX = 0 Or (Not PreviousNodeY = 11 And Not PreviousNodeX = 13 And Not PreviousNodeX = 12) Then

            GhostPath = FindPathBFS(IX, IY, PreviousNodeY, PreviousNodeX)

            If GhostPath IsNot Nothing AndAlso Not GhostPath.Count = 1 Then

                'Replaces Previous tile with most Previous tile
                PreviousTileX = GhostPath(0).Y
                PreviousTileY = GhostPath(0).X

                GhostPath.Dequeue()

                'Moves Ghost to next location 
                IX = GhostPath(0).Y
                IY = GhostPath(0).X


            Else

                'If the Ghost has recently been activated and is changing it makes sure that 
                Dim TempIX = PreviousTileX
                Dim TenpIY = PreviousTileY

                PreviousTileX = IX
                PreviousTileY = IY

                IX = TempIX
                IY = TenpIY


            End If
        End If

        changing = False

    End Sub

    Protected Sub UpdateGhostdirection()

        'Updates the number of the new Ghost direction its looking at

        If PreviousTileX = IX - 1 And PreviousTileY = IY Then
            BackDirection = 37

            Direction = 39 'Left
        ElseIf PreviousTileX = IX + 1 And PreviousTileY = IY Then
            BackDirection = 39

            Direction = 37 'Right 
        ElseIf PreviousTileX = IX And PreviousTileY = IY - 1 Then
            BackDirection = 38

            Direction = 40 'Down 
        ElseIf PreviousTileX = IX And PreviousTileY = IY + 1 Then
            BackDirection = 40

            Direction = 38 'Up
        End If

    End Sub

    Protected Function CheckIfChangingIsTrue()

        If Notchanging = True And changing = True Then
            Notchanging = False
            Return False
        ElseIf changing = True Then
            Return True
        Else
            Return False
        End If

    End Function

#End Region

End Class
Public Class Redghost
    Inherits Ghost
    Public Sub New(startX As Integer, startY As Integer)
        MyBase.New(startX, startY)
    End Sub

    Public Overrides Sub Move(map(,) As Integer)

        'Ends sub is the Ghost has not been activated by GhostActivationTimer_Tick
        If Not IsActive = True Then
            Exit Sub
        End If

        changing = CheckIfChangingIsTrue()

        'Makes sure that Ghost x coord and y coord is within the map 
        'And Portal checks

        If Not (IX <= 0 Or IX >= 27 Or IY <= 0 Or IY >= 30) AndAlso PortalChecksFunction() = False Then

            If Eaten = True Then

                'Eaten mode 
                'Targets middle location of Ghost box

                EatenUpdateProtocalSub()

            ElseIf frightened = True And changing = False Then

                'Frightened mode 
                'Gernerates the random direction and path the Ghost follows when in the frightened mode
                'Updates the directions the Ghost it currently looking in

                'Sub gets ended becuase path has already been generated

                FrightednedUpdateProtocalSub()
                UpdateGhostdirection()

                Exit Sub

            ElseIf Scatter = True And changing = False Then

                'Scatter mode 
                'Targets correct (top right corner) scatter square for the red Ghost

                TopRightScatterSQProtocal()

            End If

        End If

        'Chase mode
        If changing = False Then
            'Ghost has not swaped modes 
            GhostChaseProtocalSub()
        Else
            'The Ghost is currently swaping into another mode
            GhostChangingToOppositeDirectionSub()
        End If

        'Updates the directions the Ghost it currently looking in
        UpdateGhostdirection()

    End Sub

    'Generates the Ghost UI
    Public Overrides Sub Draw(g As Graphics, tileSize As Integer)
        Dim RGhostRect As New Rectangle(IX * tileSize, IY * tileSize, tileSize, tileSize)
        If frightened = False Then
            g.FillEllipse(Brushes.Red, RGhostRect)
        Else
            g.FillEllipse(Brushes.RoyalBlue, RGhostRect)
        End If
    End Sub

    Public Overrides Sub DrawOverPreviousLocation(g As Graphics, tileSize As Integer, MapDisplay(,) As Integer, Paint As Boolean, rest As Boolean)

        Dim entityRect As New Rectangle(PreviousTileX * tileSize, PreviousTileY * tileSize, tileSize, tileSize)

        If Not PreviousTileX = 0 And Not PreviousTileY = 0 Then

            Select Case MapDisplay(PreviousTileY, PreviousTileX)
                Case 0 'Empty space
                    g.FillRectangle(Brushes.Black, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case 1 'Pellet
                    g.FillRectangle(Brushes.Black, entityRect)
                    If Paint = True Then
                        If frightened = False Then
                            g.FillEllipse(Brushes.Red, entityRect)
                        Else
                            g.FillEllipse(Brushes.RoyalBlue, entityRect)
                        End If
                    End If
                    If rest = False Then
                        Dim pelletRect As New Rectangle(entityRect.X + tileSize \ 4, entityRect.Y + tileSize \ 4, tileSize \ 2, tileSize \ 2)
                        g.FillEllipse(Brushes.Yellow, pelletRect)
                        g.DrawRectangle(Pens.Black, entityRect)
                    End If
                    Exit Sub
                Case 2 'Wall
                    g.FillRectangle(Brushes.Blue, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case -1 'Pellet portal
                    g.FillRectangle(Brushes.Red, entityRect)
                    If Paint = True Then
                        If frightened = False Then
                            g.FillEllipse(Brushes.Red, entityRect)
                        Else
                            g.FillEllipse(Brushes.RoyalBlue, entityRect)
                        End If
                    End If
                    If rest = False Then
                        Dim pelletRect As New Rectangle(entityRect.X + tileSize \ 4, entityRect.Y + tileSize \ 4, tileSize \ 2, tileSize \ 2)
                        g.FillEllipse(Brushes.Yellow, pelletRect)
                        g.DrawRectangle(Pens.Black, entityRect)
                    End If
                    Exit Sub
                Case -2 'Empty portal 
                    g.FillRectangle(Brushes.Red, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case 3 'Mega Pellet 
                    g.FillRectangle(Brushes.Black, entityRect)
                    If Paint = True Then
                        If frightened = False Then
                            g.FillEllipse(Brushes.Red, entityRect)
                        Else
                            g.FillEllipse(Brushes.RoyalBlue, entityRect)
                        End If
                    End If
                    If rest = False Then
                        Dim pelletRect As New Rectangle(entityRect.X + tileSize \ 8, entityRect.Y + tileSize \ 8, 4 * tileSize / 5, 4 * tileSize / 5)
                        g.FillEllipse(Brushes.Yellow, pelletRect)
                        g.DrawRectangle(Pens.Black, entityRect)
                    End If
                    Exit Sub
                Case 4 'Ghost spawn
                    g.FillRectangle(Brushes.Black, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case 5 'Ghost gate 
                    g.FillRectangle(Brushes.Gray, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
            End Select

            If Paint = True Then
                If frightened = False Then
                    g.FillEllipse(Brushes.Red, entityRect)
                Else
                    g.FillEllipse(Brushes.RoyalBlue, entityRect)
                End If
            End If

        End If

    End Sub

End Class

Public Class PinkGhost
    Inherits Ghost

    Public PacmanDirection As Integer
    Public Overrides Sub Move(map(,) As Integer)

        'Ends sub is the Ghost has not been activated by GhostActivationTimer_Tick
        If Not IsActive = True Then
            Exit Sub
        End If

        changing = CheckIfChangingIsTrue()

        'Makes sure that Ghost x coord and y coord is within the map 
        'And Portal checks

        If Not (IX <= 0 Or IX >= 27 Or IY <= 0 Or IY >= 30) AndAlso PortalChecksFunction() = False Then

            If Eaten = True Then

                'Eaten mode 
                'Targets middle location of Ghost box

                EatenUpdateProtocalSub()

            ElseIf frightened = True And changing = False Then 'Frightened mode 

                'Frightened mode 
                'Gernerates the random direction and path the Ghost follows when in the frightened mode
                'Updates the directions the Ghost it currently looking in

                'Sub gets ended becuase path has already been generated

                FrightednedUpdateProtocalSub()
                UpdateGhostdirection()

                Exit Sub

            ElseIf Scatter = True And changing = False Then

                'Scatter mode 
                'Targets correct (top right corner) scatter square for the red Ghost
                TopLeftScatterSQProtocal()

            End If

        End If

        'Chase mode
        If changing = False Then

            'Ghost has not swaped modes 
            'Case giveing Pink Ghost personality to track infront of Pacman

            ''Start of Pinkys personality (track 4 tiles in frount of Pacman)

            If frightened = False And Eaten = False And Scatter = False Then

                Select Case PacmanDirection
                    Case 37 'Left

                        Dim TempIX As Integer
                        Dim OriginalTIX As Integer = TrackingIX

                        For n As Integer = 1 To 4

                            TempIX = OriginalTIX - n

                            If TempIX < 1 Then
                                Exit For
                            End If

                            If GhostMaps(MapNo)(TrackingIY, TempIX) = 1 Then
                                TrackingIX = TempIX
                            End If
                        Next

                    Case 38 'Up

                        Dim TempIY As Integer
                        Dim OriginalTIY As Integer = TrackingIY

                        For n As Integer = 1 To 4

                            TempIY = OriginalTIY - n

                            If TempIY < 1 Then
                                Exit For
                            End If

                            If GhostMaps(MapNo)(TempIY, TrackingIX) = 1 Then
                                TrackingIY = TempIY
                            End If
                        Next

                    Case 39 'Right

                        Dim TempIX As Integer
                        Dim OriginalTIX As Integer = TrackingIX

                        For n As Integer = 1 To 4

                            TempIX = OriginalTIX + n

                            If TempIX > 26 Then
                                Exit For
                            End If

                            If GhostMaps(MapNo)(TrackingIY, TempIX) = 1 Then
                                TrackingIX = TempIX
                            End If
                        Next

                    Case 40 'Down

                        Dim TempIY As Integer
                        Dim OriginalTIY As Integer = TrackingIY

                        For n As Integer = 1 To 4

                            TempIY = OriginalTIY + n

                            If TempIY > 29 Then
                                Exit For
                            End If

                            If GhostMaps(MapNo)(TempIY, TrackingIX) = 1 Then
                                TrackingIY = TempIY
                            End If
                        Next

                End Select

            End If

            'Makes sure that Ghost dosnt get stuck on target tile 
            If Not IX <> TrackingIX And Not IY <> TrackingIY Then
                TrackingIX = PreviousNodeX
                TrackingIY = PreviousNodeY
            End If

            ''End of Pinkys personality 

            GhostChaseProtocalSub()

        ElseIf changing = True Then

            'The Ghost is currently swaping into another mode
            GhostChangingToOppositeDirectionSub()

        End If

        'Updates the directions the Ghost it currently looking in
        UpdateGhostdirection()


    End Sub
    Public Sub New(startX As Integer, startY As Integer)
        MyBase.New(startX, startY)
    End Sub
    Public Overrides Sub Draw(g As Graphics, tileSize As Integer)
        Dim PGhostRect As New Rectangle(IX * tileSize, IY * tileSize, tileSize, tileSize)
        If frightened = False Then
            g.FillEllipse(Brushes.Pink, PGhostRect)
        Else
            g.FillEllipse(Brushes.RoyalBlue, PGhostRect)
        End If
    End Sub

    Public Overrides Sub DrawOverPreviousLocation(g As Graphics, tileSize As Integer, MapDisplay(,) As Integer, paint As Boolean, rest As Boolean)

        If Not PreviousTileX = 0 Or Not PreviousTileY = 0 Then

            Dim entityRect As New Rectangle(PreviousTileX * tileSize, PreviousTileY * tileSize, tileSize, tileSize)

            Select Case MapDisplay(PreviousTileY, PreviousTileX)
                Case 0 'Empty space
                    g.FillRectangle(Brushes.Black, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case 1 'Pellet
                    g.FillRectangle(Brushes.Black, entityRect)
                    If paint = True Then
                        If frightened = False Then
                            g.FillEllipse(Brushes.Pink, entityRect)
                        Else
                            g.FillEllipse(Brushes.RoyalBlue, entityRect)
                        End If
                    End If
                    If rest = False Then
                        Dim pelletRect As New Rectangle(entityRect.X + tileSize \ 4, entityRect.Y + tileSize \ 4, tileSize \ 2, tileSize \ 2)
                        g.FillEllipse(Brushes.Yellow, pelletRect)
                        g.DrawRectangle(Pens.Black, entityRect)
                    End If
                    Exit Sub
                Case 2 'Wall
                    g.FillRectangle(Brushes.Blue, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case -1 'Pellet portal
                    g.FillRectangle(Brushes.Red, entityRect)
                    If paint = True Then
                        If frightened = False Then
                            g.FillEllipse(Brushes.Pink, entityRect)
                        Else
                            g.FillEllipse(Brushes.RoyalBlue, entityRect)
                        End If
                    End If
                    If rest = False Then
                        Dim pelletRect As New Rectangle(entityRect.X + tileSize \ 4, entityRect.Y + tileSize \ 4, tileSize \ 2, tileSize \ 2)
                        g.FillEllipse(Brushes.Yellow, pelletRect)
                        g.DrawRectangle(Pens.Black, entityRect)
                    End If
                    Exit Sub
                Case -2 'Empty portal 
                    g.FillRectangle(Brushes.Red, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case 3 'Mega Pellet 
                    g.FillRectangle(Brushes.Black, entityRect)
                    If paint = True Then
                        If frightened = False Then
                            g.FillEllipse(Brushes.Pink, entityRect)
                        Else
                            g.FillEllipse(Brushes.RoyalBlue, entityRect)
                        End If
                    End If
                    If rest = False Then
                        Dim pelletRect As New Rectangle(entityRect.X + tileSize \ 8, entityRect.Y + tileSize \ 8, 4 * tileSize / 5, 4 * tileSize / 5)
                        g.FillEllipse(Brushes.Yellow, pelletRect)
                        g.DrawRectangle(Pens.Black, entityRect)
                    End If
                    Exit Sub
                Case 4 'Ghost spawn
                    g.FillRectangle(Brushes.Black, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case 5 'Ghost gate 
                    g.FillRectangle(Brushes.Gray, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
            End Select

            If paint = True Then
                If frightened = False Then
                    g.FillEllipse(Brushes.Pink, entityRect)
                Else
                    g.FillEllipse(Brushes.RoyalBlue, entityRect)
                End If
            End If

        End If

    End Sub

End Class

Public Class BlueGhost
    Inherits Ghost

    Public Sub New(startX As Integer, startY As Integer)
        MyBase.New(startX, startY)
    End Sub

    Public PacmanDirection As Integer
    Public RedghostIX As Integer
    Public RedghostIY As Integer
    Public Overrides Sub Move(map(,) As Integer)

        'Ends sub is the Ghost has not been activated by GhostActivationTimer_Tick
        If Not IsActive = True Then

            Exit Sub

        ElseIf JustActivated = True Then

            Dim TempQueue As New Queue(Of Tile)

            TempQueue.Enqueue(New Tile(14, 13))
            TempQueue.Enqueue(New Tile(13, 13))
            TempQueue.Enqueue(New Tile(12, 13))
            TempQueue.Enqueue(New Tile(11, 13))

            GhostPath = TempQueue

            Notchanging = True
            JustActivated = False

        End If

        changing = CheckIfChangingIsTrue()

        'Makes sure that Ghost x coord and y coord is within the map 
        'And Portal checks

        If Not (IX <= 0 Or IX >= 27 Or IY <= 0 Or IY >= 30) AndAlso PortalChecksFunction() = False Then

            If Eaten = True Then

                'Eaten mode 
                'Targets middle location of Ghost box

                EatenUpdateProtocalSub()

            ElseIf frightened = True And changing = False Then 'Frightened mode 

                'Frightened mode 
                'Gernerates the random direction and path the Ghost follows when in the frightened mode
                'Updates the directions the Ghost it currently looking in

                'Sub gets ended becuase path has already been generated

                FrightednedUpdateProtocalSub()
                UpdateGhostdirection()

                Exit Sub

            ElseIf Scatter = True And changing = False Then

                'Scatter mode 
                'Targets correct (top right corner) scatter square for the red Ghost
                BottomRightScatterSQProtocal()

            End If

        End If

        'Chase mode
        If changing = False Then

            If frightened = False And Eaten = False And Scatter = False Then

                'Ghost has not swaped modes 
                'Calculation giveing Blue Ghost personality of working togever with blinky to get Pacman?
                'The blue Ghost uses pacmans location and teh red ghosts location to find its tracking target tile 
                'An intermideat tile is found
                'The vector from Pacman to the red Ghost it calculated 
                'The vector gets flipped 180d and thats the tracking tile 



                Dim IntermediateTileIX As Integer
                Dim IntermediateTileIY As Integer

                Select Case PacmanDirection
                    Case 37 'Left

                        Dim TempIX As Integer
                        Dim OriginalTIX As Integer = TrackingIX
                        IntermediateTileIY = TrackingIY

                        For n As Integer = 0 To 2

                            TempIX = OriginalTIX - n

                            If TempIX < 1 Then
                                Exit For
                            End If

                            If GhostMaps(MapNo)(TrackingIY, TempIX) = 1 Then
                                IntermediateTileIX = TempIX
                            End If
                        Next

                    Case 38 'Up

                        Dim TempIY As Integer
                        Dim OriginalTIY As Integer = TrackingIY
                        IntermediateTileIX = TrackingIX

                        For n As Integer = 0 To 2

                            TempIY = OriginalTIY - n

                            If TempIY < 1 Then
                                Exit For
                            End If

                            If GhostMaps(MapNo)(TempIY, TrackingIX) = 1 Then
                                IntermediateTileIY = TempIY
                            End If
                        Next

                    Case 39 'Right

                        Dim TempIX As Integer
                        Dim OriginalTIX As Integer = TrackingIX
                        IntermediateTileIY = TrackingIY

                        For n As Integer = 0 To 2

                            TempIX = OriginalTIX + n

                            If TempIX > 26 Then
                                Exit For
                            End If

                            If GhostMaps(MapNo)(TrackingIY, TempIX) = 1 Then
                                IntermediateTileIX = TempIX
                            End If
                        Next

                    Case 40 'Down

                        Dim TempIY As Integer
                        Dim OriginalTIY As Integer = TrackingIY
                        IntermediateTileIX = TrackingIX

                        For n As Integer = 0 To 2

                            TempIY = OriginalTIY + n

                            If TempIY > 29 Then
                                Exit For
                            End If

                            If GhostMaps(MapNo)(TempIY, TrackingIX) = 1 Then
                                IntermediateTileIY = TempIY
                            End If
                        Next
                End Select

                Dim VectorIX As Integer
                Dim VectorIY As Integer

                VectorIX = IntermediateTileIX - RedghostIX
                VectorIY = IntermediateTileIY - RedghostIY

                TrackingIX = IntermediateTileIX
                TrackingIY = IntermediateTileIY

                TrackingIX += VectorIX
                TrackingIY += VectorIY

                If TrackingIX > 26 Then
                    TrackingIX = 26
                ElseIf TrackingIX < 1 Then
                    TrackingIX = 1
                End If
                If TrackingIY > 29 Then
                    TrackingIY = 29
                ElseIf TrackingIY < 1 Then
                    TrackingIY = 1
                End If

                If Not GhostMaps(MapNo)(TrackingIY, TrackingIX) = 1 Then
                    Do Until GhostMaps(MapNo)(TrackingIY, TrackingIX) = 1
                        If Math.Abs(VectorIX) > Math.Abs(VectorIY) Then
                            If VectorIX > 0 Then
                                VectorIX -= 1
                                TrackingIX -= 1
                            ElseIf VectorIX < 0 Then
                                VectorIX += 1
                                TrackingIX += 1
                            End If
                        Else
                            If VectorIY > 0 Then
                                VectorIY -= 1
                                TrackingIY -= 1
                            ElseIf VectorIY < 0 Then
                                VectorIY += 1
                                TrackingIY += 1
                            End If
                        End If
                    Loop
                End If

            End If


            ''End of Inkys personality 
            GhostChaseProtocalSub()

        ElseIf changing = True Then

            'The Ghost is currently swaping into another mode
            GhostChangingToOppositeDirectionSub()

        End If

        'Updates the directions the Ghost it currently looking in
        UpdateGhostdirection()


    End Sub

    Public Overrides Sub Draw(g As Graphics, tileSize As Integer)
        Dim BGhostRect As New Rectangle(IX * tileSize, IY * tileSize, tileSize, tileSize)
        If frightened = False Then
            g.FillEllipse(Brushes.LightBlue, BGhostRect)
        Else
            g.FillEllipse(Brushes.RoyalBlue, BGhostRect)
        End If
    End Sub

    Public Overrides Sub DrawOverPreviousLocation(g As Graphics, tileSize As Integer, MapDisplay(,) As Integer, paint As Boolean, rest As Boolean)

        If Not PreviousTileX = 0 Or Not PreviousTileY = 0 Then

            Dim entityRect As New Rectangle(PreviousTileX * tileSize, PreviousTileY * tileSize, tileSize, tileSize)

            Select Case MapDisplay(PreviousTileY, PreviousTileX)
                Case 0 'Empty space
                    g.FillRectangle(Brushes.Black, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case 1 'Pellet
                    g.FillRectangle(Brushes.Black, entityRect)
                    If paint = True Then
                        If frightened = False Then
                            g.FillEllipse(Brushes.LightBlue, entityRect)
                        Else
                            g.FillEllipse(Brushes.RoyalBlue, entityRect)
                        End If
                    End If
                    If rest = False Then
                        Dim pelletRect As New Rectangle(entityRect.X + tileSize \ 4, entityRect.Y + tileSize \ 4, tileSize \ 2, tileSize \ 2)
                        g.FillEllipse(Brushes.Yellow, pelletRect)
                        g.DrawRectangle(Pens.Black, entityRect)
                    End If
                    Exit Sub
                Case 2 'Wall
                    g.FillRectangle(Brushes.Blue, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case -1 'Pellet portal
                    g.FillRectangle(Brushes.Red, entityRect)
                    If paint = True Then
                        If frightened = False Then
                            g.FillEllipse(Brushes.LightBlue, entityRect)
                        Else
                            g.FillEllipse(Brushes.RoyalBlue, entityRect)
                        End If
                    End If
                    If rest = False Then
                        Dim pelletRect As New Rectangle(entityRect.X + tileSize \ 4, entityRect.Y + tileSize \ 4, tileSize \ 2, tileSize \ 2)
                        g.FillEllipse(Brushes.Yellow, pelletRect)
                        g.DrawRectangle(Pens.Black, entityRect)
                    End If
                    Exit Sub
                Case -2 'Empty portal 
                    g.FillRectangle(Brushes.Red, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case 3 'Mega Pellet 
                    g.FillRectangle(Brushes.Black, entityRect)
                    If paint = True Then
                        If frightened = False Then
                            g.FillEllipse(Brushes.LightBlue, entityRect)
                        Else
                            g.FillEllipse(Brushes.RoyalBlue, entityRect)
                        End If
                    End If
                    If rest = False Then
                        Dim pelletRect As New Rectangle(entityRect.X + tileSize \ 8, entityRect.Y + tileSize \ 8, 4 * tileSize / 5, 4 * tileSize / 5)
                        g.FillEllipse(Brushes.Yellow, pelletRect)
                        g.DrawRectangle(Pens.Black, entityRect)
                    End If
                    Exit Sub
                Case 4 'Ghost spawn
                    g.FillRectangle(Brushes.Black, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case 5 'Ghost gate 
                    g.FillRectangle(Brushes.Gray, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
            End Select

            If paint = True Then
                If frightened = False Then
                    g.FillEllipse(Brushes.LightBlue, entityRect)
                Else
                    g.FillEllipse(Brushes.RoyalBlue, entityRect)
                End If
            End If

        End If

    End Sub

End Class
Public Class OrangeGhost
    Inherits Ghost

    Public Sub New(startX As Integer, startY As Integer)
        MyBase.New(startX, startY)
    End Sub

    Public Overrides Sub Move(map(,) As Integer)

        'Ends sub is the Ghost has not been activated by GhostActivationTimer_Tick
        If Not IsActive = True Then

            Exit Sub

        ElseIf JustActivated = True Then

            Dim TempQueue As New Queue(Of Tile)

            TempQueue.Enqueue(New Tile(14, 13))
            TempQueue.Enqueue(New Tile(13, 13))
            TempQueue.Enqueue(New Tile(12, 13))
            TempQueue.Enqueue(New Tile(11, 13))

            GhostPath = TempQueue

            Notchanging = True
            JustActivated = False

        End If

        changing = CheckIfChangingIsTrue()

        'Makes sure that Ghost x coord and y coord is within the map 
        'And Portal checks

        If Not (IX <= 0 Or IX >= 27 Or IY <= 0 Or IY >= 30) AndAlso PortalChecksFunction() = False Then

            If Eaten = True Then

                'Eaten mode 
                'Targets middle location of Ghost box

                EatenUpdateProtocalSub()

            ElseIf frightened = True And changing = False Then 'Frightened mode 

                'Frightened mode 
                'Gernerates the random direction and path the Ghost follows when in the frightened mode
                'Updates the directions the Ghost it currently looking in

                'Sub gets ended becuase path has already been generated

                FrightednedUpdateProtocalSub()
                UpdateGhostdirection()

                Exit Sub

            ElseIf Scatter = True And changing = False Then

                'Scatter mode 
                'Targets correct (top right corner) scatter square for the red Ghost
                BottomRightScatterSQProtocal()

            End If

        End If

        'Chase mode
        If changing = False Then

            'Ghost has not swaped modes 
            'Calculation giveing Orange Ghost personality of being scared of Pacman?
            'Using triginomity to see if Pacman is 8 tiles away from orange Ghost 

            ''Start of Clydes personality

            If frightened = False And Eaten = False And Scatter = False Then

                Dim TempIX As Integer = Math.Abs(IX - TrackingIX)
                Dim TempIY As Integer = Math.Abs(IY - TrackingIY)

                Dim distance As Integer = Math.Ceiling(Math.Sqrt(TempIY ^ 2 + TempIX ^ 2))

                If distance <= 8 Then
                    TrackingIX = TrackingSQIX(0)
                    TrackingIY = TrackingSQIY(0)
                End If

            End If

            ''End of Clydes personality 

            GhostChaseProtocalSub()

        ElseIf changing = True Then

            'The Ghost is currently swaping into another mode
            GhostChangingToOppositeDirectionSub()

        End If

        'Updates the directions the Ghost it currently looking in
        UpdateGhostdirection()


    End Sub

    Public Overrides Sub Draw(g As Graphics, tileSize As Integer)
        Dim OGhostRect As New Rectangle(IX * tileSize, IY * tileSize, tileSize, tileSize)
        If frightened = False Then
            g.FillEllipse(Brushes.Orange, OGhostRect)
        Else
            g.FillEllipse(Brushes.RoyalBlue, OGhostRect)
        End If
    End Sub

    Public Overrides Sub DrawOverPreviousLocation(g As Graphics, tileSize As Integer, MapDisplay(,) As Integer, paint As Boolean, rest As Boolean)

        If Not PreviousTileX = 0 Or Not PreviousTileY = 0 Then

            Dim entityRect As New Rectangle(PreviousTileX * tileSize, PreviousTileY * tileSize, tileSize, tileSize)

            Select Case MapDisplay(PreviousTileY, PreviousTileX)
                Case 0 'Empty space
                    g.FillRectangle(Brushes.Black, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case 1 'Pellet
                    g.FillRectangle(Brushes.Black, entityRect)
                    If paint = True Then
                        If frightened = False Then
                            g.FillEllipse(Brushes.Orange, entityRect)
                        Else
                            g.FillEllipse(Brushes.RoyalBlue, entityRect)
                        End If
                    End If
                    If rest = False Then
                        Dim pelletRect As New Rectangle(entityRect.X + tileSize \ 4, entityRect.Y + tileSize \ 4, tileSize \ 2, tileSize \ 2)
                        g.FillEllipse(Brushes.Yellow, pelletRect)
                        g.DrawRectangle(Pens.Black, entityRect)
                    End If
                    Exit Sub
                Case 2 'Wall
                    g.FillRectangle(Brushes.Blue, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case -1 'Pellet portal
                    g.FillRectangle(Brushes.Red, entityRect)
                    If paint = True Then
                        If frightened = False Then
                            g.FillEllipse(Brushes.Orange, entityRect)
                        Else
                            g.FillEllipse(Brushes.RoyalBlue, entityRect)
                        End If
                    End If
                    If rest = False Then
                        Dim pelletRect As New Rectangle(entityRect.X + tileSize \ 4, entityRect.Y + tileSize \ 4, tileSize \ 2, tileSize \ 2)
                        g.FillEllipse(Brushes.Yellow, pelletRect)
                        g.DrawRectangle(Pens.Black, entityRect)
                    End If
                    Exit Sub
                Case -2 'Empty portal 
                    g.FillRectangle(Brushes.Red, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case 3 'Mega Pellet 
                    g.FillRectangle(Brushes.Black, entityRect)
                    If paint = True Then
                        If frightened = False Then
                            g.FillEllipse(Brushes.Orange, entityRect)
                        Else
                            g.FillEllipse(Brushes.RoyalBlue, entityRect)
                        End If
                    End If
                    If rest = False Then
                        Dim pelletRect As New Rectangle(entityRect.X + tileSize \ 8, entityRect.Y + tileSize \ 8, 4 * tileSize / 5, 4 * tileSize / 5)
                        g.FillEllipse(Brushes.Yellow, pelletRect)
                        g.DrawRectangle(Pens.Black, entityRect)
                    End If
                    Exit Sub
                Case 4 'Ghost spawn
                    g.FillRectangle(Brushes.Black, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
                Case 5 'Ghost gate 
                    g.FillRectangle(Brushes.Gray, entityRect)
                    g.DrawRectangle(Pens.Black, entityRect)
            End Select
            If paint = True Then
                If frightened = False Then
                    g.FillEllipse(Brushes.Orange, entityRect)
                Else
                    g.FillEllipse(Brushes.RoyalBlue, entityRect)
                End If
            End If
        End If

    End Sub

End Class

' add new ghosts here 

'Tiles for BFS
Public Class Tile
    Public X As Integer
    Public Y As Integer
    Public Parent As Tile

    'Adds new neigbor tile
    Public Sub New(x As Integer, y As Integer)
        Me.X = x
        Me.Y = y

        Me.Parent = Nothing
    End Sub
End Class

'Weighted nodes for Dijkstras 
Public Class Node
    Public X As Integer
    Public Y As Integer
    Public G As Integer
    Public W As Integer
    Public Parent As Node

    'Adds new neigbor node
    Public Sub New(x As Integer, y As Integer, w As Integer)
        Me.X = x
        Me.Y = y
        Me.G = 0
        Me.W = w

        Me.Parent = Nothing
    End Sub

End Class
