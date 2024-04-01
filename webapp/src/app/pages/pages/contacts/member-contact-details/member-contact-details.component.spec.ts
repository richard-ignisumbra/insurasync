import { ComponentFixture, TestBed } from "@angular/core/testing";

import { MemberContactDetailsComponent } from "./member-contact-details.component";

describe("MemberListComponent", () => {
  let component: MemberContactDetailsComponent;
  let fixture: ComponentFixture<MemberContactDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MemberContactDetailsComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberContactDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
