namespace ServiceDesignCompanion.Site.Content;

/// <summary>
/// One point of the UK Service Standard. Titles are reused from GOV.UK under the Open Government
/// Licence v3.0; summaries, explainers and questions are written for this app.
/// </summary>
public sealed record StandardPoint(
    int Number,
    string Title,
    string GuidanceSlug,
    string Summary = "",
    string ExplainerMarkup = "",
    IReadOnlyList<string>? Questions = null)
{
    public string GuidanceUrl => $"https://www.gov.uk/service-manual/service-standard/{GuidanceSlug}";
}

public static class ServiceStandard
{
    public const string SourceUrl = "https://www.gov.uk/service-manual/service-standard";

    public static readonly IReadOnlyList<StandardPoint> Points =
    [
        new(1, "Understand users and their needs", "point-1-understand-user-needs",
            Summary: "Find out who your users are, what they are trying to get done and the problem behind it, before you settle on a solution.",
            ExplainerMarkup: """
                <p>Start from the problem people have, not from a service someone has already imagined. Research the whole of what people are trying to do, including the parts that happen outside your service, and test your assumptions early, while they are still cheap to change.</p>
                <p>In practice that means regular research with the people who will use the service, including those who find things hardest; quick, throwaway prototypes to test ideas; and making use of the data you already have, such as analytics and contact centre records.</p>
                """,
            Questions:
            [
                "Who have you researched with, and who is still missing?",
                "Which of your user needs are backed by evidence, and which are still assumptions?",
                "What have you learned that surprised you or changed your plans?",
                "How will you keep learning once the service is live?",
            ]),
        new(2, "Solve a whole problem for users", "point-2-solve-a-whole-problem",
            Summary: "Scope your service around the whole of what someone is trying to do, and join it up with the services either side of yours.",
            ExplainerMarkup: """
                <p>People rarely think in terms of one department's service. They have a problem to sort out, and a service that covers only part of it leaves them to work out the rest. At the same time, a service that tries to do everything becomes too complicated to use. Aim for a scope that matches a real user need, neither too narrow nor too broad.</p>
                <p>Find out what constraints are real, such as legislation, and work with policy colleagues on the ones that get in the way. Check whether something already exists that you can reuse. Agree with the other organisations involved how your part fits into the wider journey, and avoid asking people for the same information twice.</p>
                """,
            Questions:
            [
                "Where does your user's journey start and end, and how much of it does your service cover?",
                "Which other services or organisations does the journey pass through, and have you agreed how they join up?",
                "Which of your constraints are genuinely fixed, and which could policy colleagues help change?",
                "Could part of the problem be solved without a new service, for example with better guidance or an API?",
            ]),
        new(3, "Provide a joined up experience across all channels", "point-3-join-up-across-channels",
            Summary: "Make sure people get the same good service whether they use it online, by phone, by post or in person.",
            ExplainerMarkup: """
                <p>Not everyone can or wants to use a website. If the offline routes are an afterthought, the people who rely on them get a worse service, and those are often the people who most need help. Joined-up channels also let staff help someone wherever they get stuck.</p>
                <p>Involve front-line and operations staff in your research and design, test the offline parts as well as the online ones, and make sure information moves between channels so nobody has to start again. Keep the ways to get help offline easy to find, and tell staff when the online service changes.</p>
                """,
            Questions:
            [
                "Which channels do people use to reach your service, and have you researched each of them?",
                "If someone starts online and then phones for help, can the person they speak to see where they got to?",
                "How do front-line staff find out about changes to the online service?",
                "What would someone without internet access need to do, and have you tried it yourself?",
            ]),
        new(4, "Make the service simple to use", "point-4-make-the-service-simple-to-use",
            Summary: "Design every part of the service so people can succeed first time, without needing help.",
            ExplainerMarkup: """
                <p>A service that is hard to use costs everyone: people make mistakes, call for help, or give up, and fixing those mistakes costs money. Unnecessary complexity also makes people trust a service less.</p>
                <p>Test with real and potential users often, and test everything they touch, including letters, emails and phone scripts, not just the screens. Make sure it works on the devices people actually use, and keep the language and design consistent with what people already know from other public services.</p>
                """,
            Questions:
            [
                "What proportion of people complete the service first time without help?",
                "When did you last watch someone use the service, and what did you change as a result?",
                "Which parts of the service have you not tested yet, such as letters or confirmation emails?",
                "Which devices and browsers do your users actually use, and does the service work well on all of them?",
            ]),
        new(5, "Make sure everyone can use the service", "point-5-make-sure-everyone-can-use-the-service",
            Summary: "Build the service so nobody in its intended audience is left out, including disabled people and people without digital skills or access.",
            ExplainerMarkup: """
                <p>Public services have to work for everyone who needs them, and public bodies have a legal duty to consider all their users. Designing inclusively also tends to make a service better for everyone, including people in a hurry or on a small screen.</p>
                <p>Meet accessibility standards for both the online and offline parts of the service, research with people who represent your whole audience, including people with access needs, and make sure there is support for people who cannot use the service online on their own.</p>
                """,
            Questions:
            [
                "How many people with access needs have taken part in your research, and what did you learn from them?",
                "Have you checked the service against accessibility standards, and do you have an accessibility statement?",
                "What support is there for someone who cannot complete the service online on their own?",
                "Is any group within your audience less likely to succeed, and do you know why?",
            ]),
        new(6, "Have a multidisciplinary team", "point-6-have-a-multidisciplinary-team",
            Summary: "Bring together the mix of skills the service needs, with the people who can make decisions working as part of the team.",
            ExplainerMarkup: """
                <p>Building and running a good service takes many skills: research, design, content, technology, policy, operations and more. When the people who make decisions are part of the team, it can act quickly on what it learns instead of waiting for approval from elsewhere.</p>
                <p>Choose roles that suit the phase you are in, and include people who understand the offline channels and the systems the service depends on. Get specialist help when you need it, such as legal or policy advice, keep reliance on suppliers sustainable, and if the service uses AI, make sure someone fully understands how it works and its effects.</p>
                """,
            Questions:
            [
                "Which skills does your team have now, and which are missing for the phase you are in?",
                "Who can make decisions about the service, and how closely do they work with the team?",
                "What happens to the team's knowledge if a key supplier or contractor leaves?",
                "If the service uses AI, who on the team understands how it reaches its results?",
            ]),
        new(7, "Use agile ways of working", "point-7-use-agile-ways-of-working",
            Summary: "Put the service in front of real users early, learn from what happens and adjust as you go.",
            ExplainerMarkup: """
                <p>Fixing every detail up front means betting on assumptions you have not tested. Working in short cycles, with real users involved early, lowers the risk of building the wrong thing and lets the service improve as the team learns.</p>
                <p>Check progress regularly and change direction when the evidence says so. Make sure governance supports this way of working, keeping the right people informed at the right level of detail, and where it makes sense, show the service to senior stakeholders such as a minister or chief executive.</p>
                """,
            Questions:
            [
                "How often does the team put something in front of real users?",
                "When did you last change the plan because of something you learned, and how quickly could you do it?",
                "Does your governance help the team respond to evidence, or slow it down?",
                "Have senior stakeholders seen the service working, rather than only a report about it?",
            ]),
        new(8, "Iterate and improve frequently", "point-8-iterate-and-improve-frequently",
            Summary: "Keep improving the service for its whole life, not just while it is first being built.",
            ExplainerMarkup: """
                <p>A service is never finished. User needs, technology and policy all change, and a service that only gets maintained gradually falls behind. Regular, meaningful improvement keeps it useful until the day it is retired.</p>
                <p>Make sure the team has the time, money and technical flexibility to make changes often. Work with your organisation to choose the improvements that matter most, keep learning from real users, and look beyond bug fixes and security patches to genuine improvements.</p>
                """,
            Questions:
            [
                "What was the last improvement you made that users would notice, and when was it?",
                "How long does it take to get a change from idea to live?",
                "How do you decide which improvements matter most?",
                "Will the team still have the capacity to improve the service after it goes live?",
            ]),
        new(9, "Create a secure service which protects users' privacy", "point-9-create-a-secure-service",
            Summary: "Protect the personal data and information the service handles, and keep security part of the work for as long as the service runs.",
            ExplainerMarkup: """
                <p>Public services hold personal and sensitive information. Losing or exposing it harms the people it belongs to and damages trust in the service and the organisation behind it.</p>
                <p>Make sure senior leaders understand the risks, and plan and budget for security across the whole life of the service. Check the security of the software and suppliers you depend on, collect only the data you need and store it securely, keep your risk assessment up to date, and test your security controls regularly. Use research to make any security steps people have to take easy to understand.</p>
                """,
            Questions:
            [
                "What personal data does the service collect, and do you need all of it?",
                "When did you last update your risk assessment, and who is accountable for the risks?",
                "How do you check the security of third-party software and suppliers?",
                "How do you find and fix vulnerabilities once the service is live?",
            ]),
        new(10, "Define what success looks like and publish performance data", "point-10-define-success-publish-performance-data",
            Summary: "Decide how you will know the service is working, measure it across every channel, and publish what you find.",
            ExplainerMarkup: """
                <p>Without clear measures you cannot tell whether a service is solving the problem it exists for, spot problems early, or know whether a change helped. Publishing performance data makes services funded by public money open to scrutiny and comparison.</p>
                <p>Choose measures that show whether the service is solving the problem, collect data from online and offline channels, and use it to decide what to improve. Refine the measures as you learn more about your users. Central government services also publish data on the mandatory key performance indicators.</p>
                """,
            Questions:
            [
                "How will you know whether the service is solving the problem it was set up to solve?",
                "Which of your measures include offline channels as well as online ones?",
                "What decision did your performance data change recently?",
                "What do you publish about the service's performance, and where?",
            ]),
        new(11, "Choose the right tools and technology", "point-11-choose-the-right-tools-and-technology",
            Summary: "Pick technology that lets you build, change and run the service well, at a sensible cost and without locking yourself in.",
            ExplainerMarkup: """
                <p>Every technology choice is an investment that shapes how easily the service can be built, changed and run for years to come. Poor choices make change slow and expensive, or tie you to one supplier.</p>
                <p>Justify whether you build or buy, and use common platforms where they fit. Understand how your technology works, including any AI, consider the total cost of ownership, use open standards to avoid lock-in, manage the legacy systems you depend on, and think about how your choices affect users and the reliability of the service.</p>
                """,
            Questions:
            [
                "Why did you choose to build or buy each major part of the service?",
                "Could you change supplier or platform if you needed to, and what would it cost?",
                "Which legacy systems does the service depend on, and how are you managing the risk?",
                "How do your technology choices affect the people using the service?",
            ]),
        new(12, "Make new source code open", "point-12-make-new-source-code-open",
            Summary: "Write new code in the open and publish it under an open licence, unless there is a clear reason not to.",
            ExplainerMarkup: """
                <p>Public services are paid for by the public, so the code behind them should be reusable. Open code lets other teams build on your work instead of starting again, which saves money and reduces dependence on any one supplier.</p>
                <p>Work in an open repository from the start, keep secrets such as passwords and access keys out of it, and make sure your organisation keeps ownership of the code and licenses it openly. Where part of the code cannot be published, such as code linked to an unannounced policy, have a clear reason for that part specifically.</p>
                """,
            Questions:
            [
                "Is your code in a public repository, and if not, what is the reason?",
                "How do you keep passwords and keys out of the code?",
                "Who owns the intellectual property in the code you are writing?",
                "Could another team reuse your code tomorrow, and what would stop them?",
            ]),
        new(13, "Use and contribute to open standards, common components and patterns", "point-13-use-common-standards-components-patterns",
            Summary: "Reuse what already works, from standards to components and design patterns, and share what you create.",
            ExplainerMarkup: """
                <p>Reusing tested components and patterns means you do not have to solve problems that have already been solved, and people get a familiar, well-tested experience. Open standards make systems easier to connect and avoid dependence on one supplier or technology.</p>
                <p>Use open standards and common government components where you can, use APIs and authoritative sources of data, and share any new or adapted components and patterns, for example through a design system. Publish useful non-personal data in open, machine-readable formats.</p>
                """,
            Questions:
            [
                "Which common components and patterns does the service reuse, and why not others?",
                "Where have you had to create something new, and have you shared it?",
                "Which open standards does the service rely on?",
                "What data could you publish openly for others to use?",
            ]),
        new(14, "Operate a reliable service", "point-14-operate-a-reliable-service",
            Summary: "Keep the service available, fast and dependable for the people who rely on it, at whatever time they need it.",
            ExplainerMarkup: """
                <p>People expect online services to be there when they need them, and many have no choice about when that is, such as a carer who can only apply late at night. A slow or unavailable service can stop someone getting help they are entitled to.</p>
                <p>Aim for the highest availability you can, and release changes regularly with as little disruption as possible. Test properly, in an environment as close to live as you can manage, monitor the service, have a sustainable plan for when things go wrong, and watch for problems with outcomes, such as bias, as well as technical faults.</p>
                """,
            Questions:
            [
                "How available and fast has the service been over the last month, and how do you know?",
                "Can you release a change without taking the service offline?",
                "What happens out of hours when something goes wrong?",
                "How would you notice if the service was producing unfair outcomes for some people?",
            ]),
    ];
}
