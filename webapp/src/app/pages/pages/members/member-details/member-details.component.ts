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
import { Component, OnInit } from "@angular/core";

import { FormGroup } from "@angular/forms";
import { IsLoadingService } from "@service-work/is-loading";
import { LinesService } from "../../../../core/api/v1/api/lines.service";
import { MemberDetails } from "../../../../core/api/v1/model/memberDetails";
import { MembersService } from "../../../../core/api/v1/api/members.service";
import { fadeInUp400ms } from "../../../../../@vex/animations/fade-in-up.animation";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import icSmartphone from "@iconify/icons-ic/twotone-smartphone";
import icweb from "@iconify/icons-ic/web";
import { protectedResources } from "../../../../auth-config";
import { stagger60ms } from "../../../../../@vex/animations/stagger.animation";

interface InsuredLine {
  line: string;
  isChecked: boolean;
}
@Component({
  selector: "vex-member-details",
  templateUrl: "./member-details.component.html",
  styleUrls: ["./member-details.component.scss"],
  animations: [stagger60ms, fadeInUp400ms],
})
export class MemberDetailsComponent implements OnInit {
  isLoading: Observable<boolean>;
  membersEndpoint: string = protectedResources.membersApi.endpoint;
  linesEndPoint: string = protectedResources.linesApi.endpoint;
  private _memberDetails = new BehaviorSubject<MemberDetails>(null);
  readonly MemberDetails$ = this._memberDetails.asObservable();

  availableMembers: any[];
  availableLines: string[];
  memberLines: InsuredLine[];
  ColumnMode = ColumnMode;

  icMoreVert = icMoreVert;
  icSmartphone = icSmartphone;
  icweb = icweb;
  public noParentMember: boolean;
  private parametersSubscription: any;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private linesService: LinesService,
    private membersService: MembersService,
    private isLoadingService: IsLoadingService
  ) {}

  ngOnInit(): void {
    this.isLoading = this.isLoadingService.isLoading$({
      key: ["appSections", "memberDetails", "getApplicationSections"],
    });
    this.parametersSubscription = this.route.paramMap.subscribe(
      (params: ParamMap) => {
        let paramID = params.get("id");
        if (paramID) {
          this.getMemberDetails(paramID);
        }
      }
    );
    this.getMembers();
    this.getLines();
  }

  getMembers() {
    console.log(this.membersEndpoint);
    this.membersService.apiMembersGet().subscribe((returnMembers) => {
      console.log("success!");
      console.log(returnMembers);

      this.availableMembers = returnMembers.filter(
        (member) => member.parentMember === null
      );
    });
  }
  getLines() {
    this.linesService.apiLinesGet().subscribe((returnLines) => {
      this.availableLines = returnLines;
      if (this._memberDetails.value) {
        this.buildLines();
      }
    });
  }

  getMemberDetails(id: any) {
    console.log(this.membersEndpoint);
    this.membersService.apiMembersIdGet(id).subscribe((returnMember) => {
      console.log("success!");
      console.log(returnMember);
      this._memberDetails.next(returnMember);

      if (this.availableLines) {
        this.buildLines();
      }

      if (returnMember.parentMember === null) {
        this.noParentMember = true;
      }
    });
  }
  buildLines() {
    console.log("buildLines");
    this.memberLines = [];
    this.availableLines.forEach((e) => {
      this.memberLines.push({ line: e, isChecked: false });
    });
    if (
      this._memberDetails.value &&
      this._memberDetails.value.linesofCoverage
    ) {
      this._memberDetails.value.linesofCoverage.forEach((e) => {
        let existingLine = this.memberLines.find((z) => z.line === e);
        if (existingLine) {
          existingLine.isChecked = true;
        }
      });
    }
  }
  hasProperty(theList: string[], searchValue: string): boolean {
    let item = theList?.find((e) => e === searchValue);
    console.log("hasProperty", item);
    if (item) {
      return true;
    } else {
      return false;
    }
  }
  updateParentMember(option, event) {
    console.log("updateParentMember");
    if (event.checked === true) {
      let details = this._memberDetails.value;
      details.parentMember = null;
      this._memberDetails.next(details);
    }
  }
  public parentComparisonFunction(option, value): boolean {
    if (value && option) {
      return option?.memberId === value?.parentMember?.memberId;
    }
  }
  public onParentMemberChange(event) {
    console.log("selectParentMember");
    console.log(event);

    this.noParentMember = false;
  }
  public saveMember() {
    console.log("saveMember");
    let memberDetails = this._memberDetails.value;
    memberDetails.linesofCoverage = [];
    memberDetails.linesofCoverage = this.memberLines
      .filter((e) => {
        if (e.isChecked) {
          return e;
        }
      })
      .map((a) => a.line);

    this.membersService
      .apiMembersPut(memberDetails)
      .subscribe((returnMember) => {
        console.log("success");
        console.log(returnMember);
        this.router.navigate(["members"]);
      });
  }
}
