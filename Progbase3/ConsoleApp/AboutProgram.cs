using Terminal.Gui;
using System;

public class AboutProgramDialog : Dialog
{
    public AboutProgramDialog()
    {
        this.Title = "About program";
        Button okBtn = new Button(30, 16, "OK");
        okBtn.Clicked += OnAboutOk;
        this.Add(okBtn);

        int rightColumn = 25;
        int coordinateX = 2;

        Label nameOfProgram = new Label(coordinateX, 2, "Name of program:");
        Label nameOfProgramData = new Label("Course work: social network.")
        {
            X = rightColumn,
            Y = Pos.Top(nameOfProgram),

        };
        this.Add(nameOfProgram, nameOfProgramData);

        Label createdFor = new Label(coordinateX, 4, "Created for:");
        Label createdForData = new Label("Basic programming course 2")
        {
            X = rightColumn,
            Y = Pos.Top(createdFor),
        };
        this.Add(createdFor, createdForData);

        Label createdOn = new Label(coordinateX, 6, "Created on:");
        DateTime time = new DateTime(2021, 06, 04);
        Label createdOnData = new Label(time.ToString())
        {
            X = rightColumn,
            Y = Pos.Top(createdOn),
        };

        this.Add(createdOn, createdOnData);

        Label createdBy = new Label(coordinateX, 8, "Created by:");
        Label createdByData = new Label("Vika Sirenko")
        {
            X = rightColumn,
            Y = Pos.Top(createdBy),
        };
        this.Add(createdBy, createdByData);

        Label group = new Label(coordinateX, 10, "Group:");
        Label groupData = new Label("KP-03")
        {
            X = rightColumn,
            Y = Pos.Top(group),
        };
        this.Add(group, groupData);


    }
    private void OnAboutOk()
    {
        Application.RequestStop();
    }
}
