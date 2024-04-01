import { Component, Inject, OnInit } from "@angular/core";
import { MAT_DIALOG_DATA } from "@angular/material/dialog";

@Component({
  selector: "app-mathis-dialog",
  templateUrl: "./mathis-dialog.component.html",
  styleUrls: ["./mathis-dialog.component.scss"],
})
export class MathisDialogComponent implements OnInit {
  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}

  ngOnInit(): void {}
}
