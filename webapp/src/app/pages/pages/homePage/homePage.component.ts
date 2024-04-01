import { Component, OnInit } from "@angular/core";

import { fadeInUp400ms } from "../../../../@vex/animations/fade-in-up.animation";
import icBeenhere from "@iconify/icons-ic/twotone-beenhere";
import icBusinessCenter from "@iconify/icons-ic/twotone-business-center";
import icMail from "@iconify/icons-ic/twotone-mail";
import icPhoneInTalk from "@iconify/icons-ic/twotone-phone-in-talk";
import icStars from "@iconify/icons-ic/twotone-stars";
import { stagger60ms } from "../../../../@vex/animations/stagger.animation";

@Component({
  selector: "home-page",
  templateUrl: "./homePage.component.html",
  styleUrls: ["./homePage.component.scss"],
  animations: [stagger60ms, fadeInUp400ms],
})
export class HomePageComponent implements OnInit {
  icBeenhere = icBeenhere;
  icStars = icStars;
  icBusinessCenter = icBusinessCenter;
  icPhoneInTalk = icPhoneInTalk;
  icMail = icMail;

  constructor() {}

  ngOnInit() {}
}
