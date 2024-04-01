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

import { HttpClient } from "@angular/common/http";
import { Member } from "src/app/core/api/v1";
import { Router } from "@angular/router";
import icMoreVert from "@iconify/icons-ic/twotone-more-vert";
import { protectedResources } from "../../../../auth-config";

@Component({
  selector: "vex-member-list",
  templateUrl: "./member-list.component.html",
  styleUrls: ["./member-list.component.scss"],
})
export class MemberListComponent implements OnInit {
  membersEndpoint: string = protectedResources.membersApi.endpoint;
  private _Members = new BehaviorSubject<Member>(null);
  Members$ = this._Members.asObservable();
  Members: any = [];
  icMoreVert = icMoreVert;
  ColumnMode = ColumnMode;
  constructor(private http: HttpClient, private router: Router) {}

  ngOnInit(): void {
    this.getMembers();
  }

  async getMembers() {
    console.log(this.membersEndpoint);

    await this.http.get(this.membersEndpoint).subscribe((returnMembers) => {
      console.log("success!");
      console.log(returnMembers);

      this._Members.next(returnMembers);
    });
  }
  viewDetails(row: any) {
    console.log("onDetails", row);
    let id = row.memberId;
    this.router.navigate(["members/details", id]);
  }
}
