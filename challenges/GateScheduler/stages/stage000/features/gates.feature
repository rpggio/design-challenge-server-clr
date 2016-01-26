Feature: Gates
  In order to perform scheduling, the system should store the gates available at an airport.
  The endpoint for gates is '/gates'.  A gate has one string attribute, 'gate', which is the identifier for the gate.
  A list of gates can be added via POST to '/gates'. All of the gates can be retrieved by GET to '/gates'.

  Background:
    Given the system is reset

  Scenario: List gates
    Given the client adds gates:
      | gate   |
      | A1     |
      | A2     |
    When the client requests gates
    Then the response is:
      | gate   |
      | A1     |
      | A2     |
