import { ActivatedRoute, ParamMap, Router } from "@angular/router";
import { Attachment, MemberDetails } from "src/app/core/api/v1";
import { BehaviorSubject, Observable, Subject } from "rxjs";
import {
  ColumnMode,
  DatatableComponent,
  DatatableGroupHeaderDirective,
  DatatableGroupHeaderTemplateDirective,
  SelectionType,
  TableColumn,
} from "@swimlane/ngx-datatable";
import {
  Component,
  Directive,
  ElementRef,
  EventEmitter,
  OnInit,
  Output,
  TemplateRef,
  ViewChild,
} from "@angular/core";

import { AddAttachmentComponent } from "../add-attachment/add-attachment.component";
import { HttpClient } from "@angular/common/http";
import { MatDialog } from "@angular/material/dialog";
import { MembersService } from "../../../../core/api/v1/api/members.service";
import { ApplicationFileUploadService } from "../../../../core/api/v1/api/applicationFileUpload.service";
import { MemberFileUploadService } from "../../../../core/api/v1/api/memberFileUpload.service";
import { PopoverService } from "../../../../../@vex/components/popover/popover.service";
import { fadeInUp400ms } from "../../../../../@vex/animations/fade-in-up.animation";
import icFileAdd from "@iconify/icons-ic/twotone-file-upload";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import { protectedResources } from "../../../../auth-config";
import { stagger60ms } from "../../../../../@vex/animations/stagger.animation";

@Component({
  selector: "vex-member-attachments",
  templateUrl: "./member-attachments.component.html",
  styleUrls: ["./member-attachments.component.scss"],
  animations: [stagger60ms, fadeInUp400ms],
})
export class MemberAttachmentComponent implements OnInit {
  membersEndpoint: string = protectedResources.membersApi.endpoint;
  private _MemberDetails = new BehaviorSubject<MemberDetails>(null);
  MemberDetails$ = this._MemberDetails.asObservable();

  private _attachments = new BehaviorSubject<Attachment[]>(null);
  readonly Attachments$ = this._attachments.asObservable();
  icMoreVert = icMoreVert;
  ColumnMode = ColumnMode;
  memberId: number;
  noteStatus: string = "active";
  previousAttachmentStatus: string;

  @ViewChild("addAttachmentTemplate", { read: TemplateRef })
  addAttachmentTemplate: TemplateRef<any>;

  private parametersSubscription: any;
  icFileAdd = icFileAdd;
  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private popoverService: PopoverService,
    private dialog: MatDialog,
    private memberService: MembersService,
    private applicationFileUploadService: ApplicationFileUploadService,
    private memberFileUploadService: MemberFileUploadService
  ) {}
  @Output()
  selectContact: EventEmitter<number> = new EventEmitter<number>();

  ngOnInit(): void {
    this.parametersSubscription = this.route.paramMap.subscribe(
      (params: ParamMap) => {
        let paramID = params.get("id");
        if (paramID) {
          this.memberId = +paramID;
          this.getAttachments("active");
          this.getMemberDetails(this.memberId);
        }
      }
    );
  }

  public getAttachments(status) {
    this.memberService
      .apiMembersMemberIdAllAttachmentsGet(this.memberId, status)
      .subscribe((notes) => {
        this._attachments.next(notes);
      });
  }
  public changeAttachments(event) {
    console.log(event.value);
    this.previousAttachmentStatus = event.value;
    this.getAttachments(event.value);
  }
  getMemberDetails(id: any) {
    this.http
      .get(this.membersEndpoint + "/" + id)
      .subscribe((returnMembers) => {
        console.log("success!");
        console.log(returnMembers);
        this._MemberDetails.next(returnMembers);
      });
  }
  viewDetails(row: any) {
    console.log("viewDetails notes", row);

    console.log("emitting - " + row.contactID);
    // this.selectContact.emit(row.contactID);
  }
  changeStatus(attachmentId: number, newStatus: string) {
    this.memberFileUploadService
      .apiMemberFileUploadAttachmentIdChangeStatusPost(attachmentId, newStatus)
      .subscribe((data) => {
        this.getAttachments(this.previousAttachmentStatus);
      });
  }
  downloadAttachment(origin: ElementRef | HTMLElement, attachment: Attachment) {
    console.log("this should download the file", attachment);

    if (attachment.applicationId !== 0) {
      this.applicationFileUploadService
        .apiApplicationFileUploadApplicationIdElementIdRowIdDownloadGet(
          attachment.applicationId,
          attachment.elementId,
          attachment.rowId,
          attachment.fileName
        )
        .subscribe((data) => {
          console.log("sas URL", data);
          (<any>window).saveAs(data, attachment.fileName);
        });
    }
    if (attachment.attachmentId !== 0) {
      this.memberFileUploadService
        .apiMemberFileUploadAttachmentIdDownloadGet(attachment.attachmentId)
        .subscribe((data) => {
          console.log("sas URL", data);
          (<any>window).saveAs(data, attachment.originalFileName);
        });
    }
  }
  showAddAttachment(origin: ElementRef | HTMLElement, attachmentId?: number) {
    let theDialog = this.dialog.open(AddAttachmentComponent, {
      data: { memberId: this.memberId, attachmentId: attachmentId },
      width: "800px",
    });

    theDialog.afterClosed().subscribe((e) => {
      if (e && e.action && e.action == "addNewAttachment") {
        this.showAddAttachment(origin, attachmentId);
      } else {
        this.getAttachments(this.noteStatus);
      }
    });
  }
}
