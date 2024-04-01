import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import {
  ColumnMode,
  DatatableComponent,
  DatatableGroupHeaderDirective,
  DatatableGroupHeaderTemplateDirective,
  SelectionType,
  TableColumn,
} from "@swimlane/ngx-datatable";
import { Component, Injector, Input, OnInit, Inject } from "@angular/core";
import { FormControl, FormGroup } from "@angular/forms";
import { ApplicationsService } from "../../../../core/api/v1/api/applications.service";
import {
  MatDialog,
  MAT_DIALOG_DATA,
  MatDialogRef,
} from "@angular/material/dialog";

import { HttpClient } from "@angular/common/http";
import { MatSnackBar } from "@angular/material/snack-bar";
import { fadeInUp400ms } from "../../../../../@vex/animations/fade-in-up.animation";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icSmartphone from "@iconify/icons-ic/twotone-smartphone";
import icweb from "@iconify/icons-ic/web";
import { protectedResources } from "../../../../auth-config";
import { stagger60ms } from "../../../../../@vex/animations/stagger.animation";

@Component({
  selector: "vex-submit-application",
  templateUrl: "./submit-application.component.html",
  styleUrls: ["./submit-application.component.scss"],
  animations: [stagger60ms, fadeInUp400ms],
})
export class SubmitApplicationComponent implements OnInit {
  contactsEndpoint: string = protectedResources.contactsApi.endpoint;

  ColumnMode = ColumnMode;

  isSaving: boolean = false;
  icMoreVert = icMoreVert;
  icSmartphone = icSmartphone;
  icweb = icweb;
  ContactDetails: any = {};
  private _memberId: number;

  createContactForm = new FormGroup({
    name: new FormControl(""),
    email: new FormControl(""),
    password: new FormControl(""),
    confirmPassword: new FormControl(""),
  });

  private parametersSubscription: any;
  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private dialogRef: MatDialogRef<SubmitApplicationComponent>,
    @Inject(MAT_DIALOG_DATA) private applicationId: number,
    private applicationService: ApplicationsService
  ) {}

  ngOnInit(): void {}

  hasProperty(theList: string[], searchValue: string): boolean {
    let item = theList?.find((e) => e === searchValue);
    console.log("hasProperty", item);
    if (item) {
      return true;
    } else {
      return false;
    }
  }
  submitApplication() {
    this.applicationService
      .apiApplicationsApplicationIdSubmitPost(this.applicationId)
      .subscribe((res) => {
        this.snackBar.open("Application submitted successfully", "Close", {
          duration: 2000,
        });
        this.closeModal();
      });
  }
  closeModal() {
    //   this.inj.get(INIT_DATA)._service.close();
    console.log("Close modal");

    this.dialogRef.close();
    //    this.dialogRef.close(true);
  }
}
