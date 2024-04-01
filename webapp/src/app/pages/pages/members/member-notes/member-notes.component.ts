import { ActivatedRoute, ParamMap, Router } from "@angular/router";
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
import { MemberDetails, Note } from "src/app/core/api/v1";

import { AddNoteComponent } from "../add-note/add-note.component";
import { HttpClient } from "@angular/common/http";
import { MatDialog } from "@angular/material/dialog";
import { MembersService } from "../../../../core/api/v1/api/members.service";
import { PopoverService } from "../../../../../@vex/components/popover/popover.service";
import { fadeInUp400ms } from "../../../../../@vex/animations/fade-in-up.animation";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icNoteAdd from "@iconify/icons-ic/twotone-note-add";
import { protectedResources } from "../../../../auth-config";
import { stagger60ms } from "../../../../../@vex/animations/stagger.animation";

@Component({
  selector: "vex-member-notes",
  templateUrl: "./member-notes.component.html",
  styleUrls: ["./member-notes.component.scss"],
  animations: [stagger60ms, fadeInUp400ms],
})
export class MemberNotesComponent implements OnInit {
  membersEndpoint: string = protectedResources.membersApi.endpoint;
  private _MemberDetails = new BehaviorSubject<MemberDetails>(null);
  MemberDetails$ = this._MemberDetails.asObservable();
  public selectedApplicationType: string = "";

  private _notes = new BehaviorSubject<Note[]>(null);
  readonly Notes$ = this._notes.asObservable();
  icMoreVert = icMoreVert;
  ColumnMode = ColumnMode;
  memberId: number;
  noteStatus: string = "active";

  @ViewChild("addNoteTemplate", { read: TemplateRef })
  addNoteTemplate: TemplateRef<any>;

  private parametersSubscription: any;
  icNoteAdd = icNoteAdd;
  constructor(
    private http: HttpClient,
    private router: Router,
    private route: ActivatedRoute,
    private popoverService: PopoverService,
    private dialog: MatDialog,
    private memberService: MembersService
  ) {}
  @Output()
  selectContact: EventEmitter<number> = new EventEmitter<number>();

  ngOnInit(): void {
    this.parametersSubscription = this.route.paramMap.subscribe(
      (params: ParamMap) => {
        let paramID = params.get("id");
        if (paramID) {
          this.memberId = +paramID;
          this.getNotes("active");
          this.getMemberDetails(this.memberId);
        }
      }
    );
  }

  public getNotes(status) {
    this.memberService
      .apiMembersMemberIdAllnotesGet(this.memberId, status)
      .subscribe((notes) => {
        this._notes.next(notes);
      });
  }
  public changeNotes(event) {
    console.log(event.value);
    this.getNotes(event.value);
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

  showAddNote(origin: ElementRef | HTMLElement, noteId?: number) {
    let theDialog = this.dialog.open(AddNoteComponent, {
      data: { memberId: this.memberId, noteId: noteId },
      width: "600px",
    });

    theDialog.afterClosed().subscribe((e) => {
      if (e && e.action && e.action == "addNewContact") {
        this.showAddNote(origin, noteId);
      } else {
        this.getNotes(this.noteStatus);
      }
    });
  }
}
