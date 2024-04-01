import { ComponentFixture, TestBed } from "@angular/core/testing";

import { MemberContactListComponent } from "./member-contact-list.component";

describe("MemberContactListComponent", () => {
  let component: MemberContactListComponent;
  let fixture: ComponentFixture<MemberContactListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MemberContactListComponent],
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MemberContactListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it("should create", () => {
    expect(component).toBeTruthy();
  });
});
