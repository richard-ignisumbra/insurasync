# perma_Questionnaire

If it complains about existing app when you try to start.

`sudo lsof -t -i tcp:3200 | xargs kill -9`

#updating documentation

- https://www.kevinboosten.dev/how-i-use-an-openapi-spec-in-my-angular-projects

# Command:

`npm run generate:api`

# Table Documentation

## Configuration

- TableSectionId: The SectionId of the table. All elementids for that sectionid will be shown (make sure to hide the section by showinnavigation = false for that sectionid)
- sourceLabels: default values and default rows. separate default values with ||
- allowNewRows: whether to show the button to allow users to add new rows.

## FAQ

- How does it know how many rows to show for each application?
  - Initially, it shows 0, or it shows the number of rows defined in the sourceLabels column of the applicationSectionElement.
  - After you add a row, it creates an entry in ApplicationElementResponse, and each time you add a row, it increments the intResponse by 1, so if there were 10 rows, it would say 11 (it starts at 0).
- What happens if I change the order of the table elements?
  - It should organize the values appropriately, so if you switch columns around, it should smartly re-organize since it uses the elementId and not the order of the values.
