USE norecruiters_demo
GO

SET IDENTITY_INSERT [dbo].[Actions] ON
GO
INSERT INTO [dbo].[Actions]([ActionId], [Description])
  VALUES(1, 'Recruiter')
GO
INSERT INTO [dbo].[Actions]([ActionId], [Description])
  VALUES(2, 'Wrong Tag')
GO
INSERT INTO [dbo].[Actions]([ActionId], [Description])
  VALUES(3, 'Spam')
GO
INSERT INTO [dbo].[Actions]([ActionId], [Description])
  VALUES(4, 'Self-Delete')
GO
SET IDENTITY_INSERT [dbo].[Actions] OFF
GO

GO
INSERT INTO [dbo].[ContentTypes]([ContentTypeId], [Description])
  VALUES(0, 'Job')
GO
INSERT INTO [dbo].[ContentTypes]([ContentTypeId], [Description])
  VALUES(1, 'Resume')
GO

INSERT INTO [dbo].[UserTypes]([UserTypeId], [Description])
  VALUES(0, 'Company')
GO
INSERT INTO [dbo].[UserTypes]([UserTypeId], [Description])
  VALUES(1, 'Person')
GO
INSERT INTO [dbo].[UserTypes]([UserTypeId], [Description])
  VALUES(2, 'Recruiter')
GO

GO
INSERT INTO [dbo].[Postings]([PostingId], [UserId], [CreatedOn], [LastModifiedOn], [ContentTypeId], [Heading], [Views], [Deleted], [Flagged], [Published], [Active], [ContentsId], [ShortName], [ShortText])
  VALUES('1D80CF3E-B5DD-487B-96DB-4F3DC040C2B5', 'BB876D54-5932-4414-92AD-EFB7BED21123', '20090520 10:47:20.0', '20090520 11:17:19.0', 1, 'Solutions-focused, team oriented Senior Technical Support Analyst', 3, 0, 0, 1, 0, '3D1DB72E-D510-444D-9910-1436DADD44AD', 'Solutions-focusedcomma_team_oriented_Senior_Technical_Support_Analyst966204', 'Sample Resume - Information Technology   _______________________     Qualifications    Solutions-focused, team oriented Senior Technical Support Analyst with broad-based experience and hands-on skill in the successful implementation of highly effective helpdesk operations and the cost-effective management of innovative customer and technical support strategies. Proven ability to successfully analyze an organization''s critical support requirements, identify deficiencies and potential opportunitie')
GO
INSERT INTO [dbo].[Postings]([PostingId], [UserId], [CreatedOn], [LastModifiedOn], [ContentTypeId], [Heading], [Views], [Deleted], [Flagged], [Published], [Active], [ContentsId], [ShortName], [ShortText])
  VALUES('E440CEE0-9148-4E15-B6DD-D637F4760690', '49D95127-610B-43CD-9638-929E3BF94072', '20090520 10:23:38.0', '20090520 11:14:17.0', 0, 'Engineering Director - Mountain View', 2, 0, 0, 1, 0, '680FA4D7-A631-4BA9-9FC7-4B144B20131F', 'Engineering_Director_-_Mountain_View759679', 'Engineering Director - Mountain View This position is available in Mountain View, CA. The area: Engineering Management Google''s engineering teams exhibit high energy, deep technical skills and a drive to get things done. Our Engineering Managers need to be technical leaders and motivators who are comfortable leading these teams in a high-pressure, dynamic &ndash; and global &ndash; environment. Jobs are broadly defined and interact with Product Management, Sales and other groups at Google. The r')
GO
INSERT INTO [dbo].[Postings]([PostingId], [UserId], [CreatedOn], [LastModifiedOn], [ContentTypeId], [Heading], [Views], [Deleted], [Flagged], [Published], [Active], [ContentsId], [ShortName], [ShortText])
  VALUES('B6A1A9CB-BEEF-4F15-AF36-E70E9E86F082', '49D95127-610B-43CD-9638-929E3BF94072', '20090520 10:21:28.0', '20090520 10:37:13.0', 0, 'Jobs @ Google', 4, 0, 0, 1, 0, '29716364-FD60-4973-BEC4-4541C66C74DC', 'Jobs_at_Google875685', '  Let&rsquo;s work together. At Google, we understand that our worldwide success results from our globally diverse workforce. In every Google office, you will find challenging projects and smart people with potential to change the world. Googlers relish the freedom to create the next generation of web technologies in an environment designed to foster collaboration, creativity, health, and happiness. What&rsquo;s it like to work at Google?        U.S. locations Mountain ViewNew YorkSanta MonicaSa')
GO
INSERT INTO [dbo].[PostingContents]([ContentsId], [Contents])
  VALUES('3D1DB72E-D510-444D-9910-1436DADD44AD', '<strong><font face="Arial" size="2">Sample Resume - Information Technology<br />   _______________________<br /></font></strong><br /><p style="margin: 6pt 0in; text-align: center" class="JobTitle" align="center"><u><span style="text-decoration: none; font-size: 13pt; font-family: Franklin Gothic Book; font-variant: small-caps"> </span></u></p>    <p style="margin: 9pt 0in 6pt" class="MsoNormal"><strong><span style="font-size: 13pt; font-family: Franklin Gothic Book; font-variant: small-caps">Qualifications</span></strong></p>    <p style="margin-top: 6pt" class="MsoNormal"><span style="font-size: 9.5pt; font-family: Franklin Gothic Book">Solutions-focused, team oriented <strong>Senior Technical Support Analyst</strong> with broad-based experience and hands-on skill in the successful implementation of highly effective helpdesk operations and the cost-effective management of innovative customer and technical support strategies. Proven ability to successfully analyze an organization''s critical support requirements, identify deficiencies and potential opportunities, and develop innovative solutions for increasing reliability and improving productivity. A broad understanding of computer hardware and software, including installation, configuration, management, troubleshooting, and support.</span></p>    <p style="margin: 9pt 0in 6pt" class="MsoNormal"><strong><span style="font-size: 13pt; font-family: Franklin Gothic Book; font-variant: small-caps">Technical</span></strong><span style="font-size: 13pt; font-family: Franklin Gothic Book; font-variant: small-caps">  <strong>Skills</strong></span></p>    <p align="center"><font face="Arial" size="2">Linux/Unix &middot; Windows 9x/NT/2000/XP &middot; Oracle &middot; FoxPro &middot; DBase II<br />   C &middot; C &middot; BASIC &middot; MS Office &middot; MS-Money &middot; Encarta</font></p>    <p style="margin: 12pt 0in 9pt" class="MsoNormal">&nbsp;</p>    <p style="margin: 12pt 0in 9pt" class="MsoNormal"><strong><span style="font-size: 13pt; font-family: Franklin Gothic Book; font-variant: small-caps">Professional</span></strong><span style="font-size: 13pt; font-family: Franklin Gothic Book; font-variant: small-caps">  <strong>Experience</strong></span></p>    <p style="margin-left: 0.5in" class="MsoNormal"><span style="font-size: 14pt; font-family: Franklin Gothic Book; font-variant: small-caps"><strong>Technical Support Enterprises</strong></span> <span style="font-size: 10pt; font-family: Franklin Gothic Book">-</span><span style="font-size: 10pt; font-family: Franklin Gothic Book">Wichita, Kansas                                                                                                       </span></p>    <p style="margin-left: 0.5in" class="MsoNormal"><span style="font-size: 10pt; font-family: Franklin Gothic Book">2002 - Present</span></p>    <p style="margin: 2pt 0in 2pt 0.5in" class="JobTitle"><u><span style="font-family: Franklin Gothic Book; font-weight: normal">Mentor/ Escalation Support  (8/2003- - present)</span></u></p>    <p style="margin: 2pt 0in 2pt 0.75in; text-indent: -0.25in" class="MsoNormal"><span style="font-size: 9pt; font-family: Symbol">&middot;<span style="font-family: ''Times New Roman''; font-style: normal; font-variant: normal; font-weight: normal; font-size: 7pt; line-height: normal; font-size-adjust: none; font-stretch: normal">         </span></span> <span style="font-size: 9.5pt; font-family: Franklin Gothic Book">Promoted to Mentor, handling escalation processes and mentoring other support professionals while working via phone, email, and chat.</span></p>    <p style="margin: 2pt 0in 2pt 0.5in" class="JobTitle"><u><span style="font-family: Franklin Gothic Book; font-weight: normal">Technical Support Manager for Microsoft Money account (6/2003 - 7/2003)</span></u></p>    <p style="margin: 2pt 0in 2pt 0.75in; text-indent: -0.25in" class="MsoNormal"><span style="font-size: 9pt; font-family: Symbol">&middot;<span style="font-family: ''Times New Roman''; font-style: normal; font-variant: normal; font-weight: normal; font-size: 7pt; line-height: normal; font-size-adjust: none; font-stretch: normal">         </span></span> <span style="font-size: 9.5pt; font-family: Franklin Gothic Book">Promoted from frontline support professional to second-tier technical support manager, supervising frontline phone support for Microsoft Corporation for Money, Encarta, PC Games, and other similar products.</span></p>    <p style="margin: 2pt 0in 2pt 0.75in; text-indent: -0.25in" class="MsoNormal"><span style="font-size: 9pt; font-family: Symbol">&middot;<span style="font-family: ''Times New Roman''; font-style: normal; font-variant: normal; font-weight: normal; font-size: 7pt; line-height: normal; font-size-adjust: none; font-stretch: normal">         </span></span> <span style="font-size: 9.5pt; font-family: Franklin Gothic Book">Responsible for the strategic development and implementation of cost-effective training and support solutions that are designed to provide improved productivity, streamlined operations, and faster access to critical information.</span></p>    <p style="margin: 2pt 0in 2pt 0.75in; text-indent: -0.25in" class="MsoNormal"><span style="font-size: 9pt; font-family: Symbol">&middot;<span style="font-family: ''Times New Roman''; font-style: normal; font-variant: normal; font-weight: normal; font-size: 7pt; line-height: normal; font-size-adjust: none; font-stretch: normal">         </span></span> <span style="font-size: 9.5pt; font-family: Franklin Gothic Book">Implement effective customer satisfaction strategies by identifying and eliminating the root causes of customer problems.</span></p>    <p style="margin: 2pt 0in 2pt 0.75in; text-indent: -0.25in" class="MsoNormal"><span style="font-size: 9pt; font-family: Symbol">&middot;<span style="font-family: ''Times New Roman''; font-style: normal; font-variant: normal; font-weight: normal; font-size: 7pt; line-height: normal; font-size-adjust: none; font-stretch: normal">         </span></span> <span style="font-size: 9.5pt; font-family: Franklin Gothic Book">Utilize NICE Application and AVAYA program to manage call center metrics, lead call calibrations, and perform random-sample audits on email and chat sessions.</span></p>    <p style="margin: 2pt 0in 2pt 0.5in" class="JobTitle"><u><span style="font-family: Franklin Gothic Book; font-weight: normal">Quality Monitoring Lead (12/2002 - 5/2003)</span></u></p>    <p style="margin: 2pt 0in 0.0001pt 0.75in; text-indent: -0.25in" class="MsoNormal"><span style="font-size: 9pt; font-family: Symbol">&middot;<span style="font-family: ''Times New Roman''; font-style: normal; font-variant: normal; font-weight: normal; font-size: 7pt; line-height: normal; font-size-adjust: none; font-stretch: normal">         </span></span> <span style="font-size: 9.5pt; font-family: Franklin Gothic Book">Participate in quality assurance procedures, verifying sales calls taken by the other agents, provide constructive feedback to agents, and adhere to the specific support levels that have been purchased by the client.</span></p>    <p style="margin: 2pt 0in 2pt 0.5in" class="JobTitle"><u><span style="font-family: Franklin Gothic Book; font-weight: normal">Support Professional for Chase Bank (9/2002 - 12/2002)</span></u></p>    <p style="margin: 2pt 0in 0.0001pt 0.75in; text-indent: -0.25in" class="MsoNormal"><span style="font-size: 9pt; font-family: Symbol">&middot;<span style="font-family: ''Times New Roman''; font-style: normal; font-variant: normal; font-weight: normal; font-size: 7pt; line-height: normal; font-size-adjust: none; font-stretch: normal">         </span></span> <span style="font-size: 9.5pt; font-family: Franklin Gothic Book">Provide comprehensive system support, configuration, maintenance, and training for Providian Bank and promoted value added products and services for existing clients.</span></p>    <p style="margin-top: 3pt" class="MsoNormal"><span style="font-size: 10pt; font-family: Franklin Gothic Book; font-variant: small-caps"> </span></p>    <p style="margin: 3pt 0in 0.0001pt 0.5in" class="MsoNormal"><span style="font-size: 14pt; font-family: Franklin Gothic Book; font-variant: small-caps"><strong>Micron Computers Ltd</strong>.</span><span style="font-size: 9.5pt; font-family: Franklin Gothic Book">-</span><span style="font-size: 10pt; font-family: Franklin Gothic Book">India</span><span style="font-size: 9.5pt; font-family: Franklin Gothic Book"> </span></p>    <p style="margin: 3pt 0in 0.0001pt 0.5in" class="MsoNormal"><span style="font-size: 10pt; font-family: Franklin Gothic Book">1998</span><span style="font-size: 9.5pt; font-family: Franklin Gothic Book">-</span><span style="font-size: 10pt; font-family: Franklin Gothic Book">2002</span></p>    <p style="margin: 2pt 0in 2pt 0.5in" class="JobTitle"><span style="font-family: Franklin Gothic Book">Hardware Engineer</span></p>    <p style="margin: 2pt 0in 2pt 0.75in; text-indent: -0.25in" class="MsoNormal"><span style="font-size: 9pt; font-family: Symbol">&middot;<span style="font-family: ''Times New Roman''; font-style: normal; font-variant: normal; font-weight: normal; font-size: 7pt; line-height: normal; font-size-adjust: none; font-stretch: normal">         </span></span> <span style="font-size: 9.5pt; font-family: Franklin Gothic Book">Performed hardware and software installations and provided high-level customer care, training, and technical support.</span></p>    <p style="margin: 2pt 0in 2pt 0.75in; text-indent: -0.25in" class="MsoNormal"><span style="font-size: 9pt; font-family: Symbol">&middot;<span style="font-family: ''Times New Roman''; font-style: normal; font-variant: normal; font-weight: normal; font-size: 7pt; line-height: normal; font-size-adjust: none; font-stretch: normal">         </span></span> <span style="font-size: 9.5pt; font-family: Franklin Gothic Book">Assembled and installed a wide array of computer systems, workstations, and peripheral hardware.</span></p>    <p style="text-align: center; margin-top: 2pt" class="MsoNormal" align="center"><strong><u><span style="text-decoration: none; font-size: 13pt; font-family: Franklin Gothic Book; font-variant: small-caps"> </span></u></strong></p>    <p style="margin-top: 2pt" class="MsoNormal"><strong><span style="font-size: 13pt; font-family: Franklin Gothic Book; font-variant: small-caps">Education</span></strong></p>    <p style="margin: 6pt 0in 0.0001pt 0.25in" class="MsoNormal"><strong><span style="font-size: 9.5pt; font-family: Franklin Gothic Book">Diploma in Computer Science</span></strong></p>    <p style="margin: 6pt 0in 0.0001pt 0.25in" class="MsoNormal"><span style="font-size: 9.5pt; font-family: Franklin Gothic Book; font-variant: small-caps">Independent Colleges Online - 2001</span></p>')
GO
INSERT INTO [dbo].[PostingContents]([ContentsId], [Contents])
  VALUES('29716364-FD60-4973-BEC4-4541C66C74DC', '<div id="intro"> <div class="column"> <h2>Let&rsquo;s work together.</h2> <p>At Google, we understand that our worldwide success results from our globally diverse workforce. In every Google office, you will find challenging projects and smart people with potential to change the world. Googlers relish the freedom to create the next generation of web technologies in an environment designed to foster collaboration, creativity, health, and happiness.</p> <p><a href="http://www.google.com/intl/en/jobs/lifeatgoogle/index.html">What&rsquo;s it like to work at Google</a>?</p> </div> <div class="column mainimg"> <img src="http://www.google.com/intl/en/jobs/images/main-google-sydney.jpg" alt="Googlers" /> </div> </div> <div class="column"> <div> <h3>U.S. locations</h3> <ul><li><a href="http://www.google.com/support/jobs/bin/static.py?page=why-ca-mv.html&amp;loc_id=1116&amp;dep_id=1173&amp;topic=1116">Mountain View</a></li><li><a href="http://www.google.com/support/jobs/bin/static.py?page=why-ny-ny.html&amp;loc_id=1122&amp;dep_id=1173&amp;topic=1122">New York</a></li><li><a href="http://www.google.com/support/jobs/bin/static.py?page=why-ca-sm.html&amp;loc_id=1118&amp;dep_id=1173&amp;topic=1118">Santa Monica</a></li><li><a href="http://www.google.com/support/jobs/bin/static.py?page=why-ca-sfo.html&amp;loc_id=13043&amp;dep_id=1173&amp;topic=13043">San Francisco</a></li><li><a href="http://www.google.com/support/jobs/bin/static.py?page=loc.html&amp;loc_id=1100&amp;dep_id=1173&amp;by_loc=1"><strong>all U.S. locations</strong></a></li></ul> </div> <div> <h3>International locations</h3> <ul><li><a href="http://www.google.co.uk/jobs">United Kingdom</a></li><li><a href="http://www.google.ch/jobs">Switzerland</a></li><li><a href="http://www.google.co.in/jobs">India</a></li><li><a href="http://www.google.ie/jobs">Ireland</a></li><li><a href="http://www.google.com/intl/en/jobs/locations.html"><strong>all international locations</strong></a></li></ul> </div> </div> <div class="column"> <h3>Learn more about &hellip;</h3> <div> <ul><li><a href="http://www.google.com/intl/en/jobs/profiles/busops.html">Business operations</a></li><li><a href="http://www.google.com/intl/en/jobs/profiles/engops.html">Engineering operations</a></li><li><a href="http://www.google.com/intl/en/jobs/profiles/finance.html">Finance</a></li><li><a href="http://www.google.com/intl/en/jobs/profiles/hr.html">Human resources</a></li><li><a href="http://www.google.com/intl/en/jobs/profiles/legal.html">Legal and public policy</a></li></ul> </div> <div> <ul><li><a href="http://www.google.com/intl/en/jobs/profiles/markcomm.html">Marketing and communications</a></li><li><a href="http://www.google.com/intl/en/jobs/profiles/product.html">Product management</a></li><li><a href="http://www.google.com/intl/en/jobs/profiles/sales.html">Sales and enterprise</a></li><li><a href="http://www.google.com/intl/en/jobs/profiles/swe.html">Software engineering</a></li><li><a href="http://www.google.com/intl/en/jobs/profiles/ux.html">User experience</a></li></ul> </div> </div> <hr /> <div class="x-note"> <p><strong>To all recruitment agencies:</strong> Google does not accept agency resumes. Please do not forward resumes to our jobs alias, Google employees or any other company location. Google is not responsible for any fees related to unsolicited resumes.</p> <p>Google is an equal opportunity employer. Employment here is based solely upon one&rsquo;s individual merit and qualifications directly related to professional competence. We don&rsquo;t discriminate on the basis of race, color, religion, national origin, ancestry, pregnancy status, sex, age, marital status, disability, medical condition, sexual orientation, gender identity, or any other characteristics protected by law. We will also make all reasonable accommodations to meet our obligations under the Americans with Disabilities Act (ADA) and state disability laws.</p> </div>')
GO
INSERT INTO [dbo].[PostingContents]([ContentsId], [Contents])
  VALUES('680FA4D7-A631-4BA9-9FC7-4B144B20131F', '<h2>Engineering Director - Mountain View</h2> <p><strong>This position is available in Mountain View, CA.</strong></p> <h3>The area: Engineering Management</h3> <p>Google''s engineering teams exhibit high energy, deep technical skills and a drive to get things done. Our Engineering Managers need to be technical leaders and motivators who are comfortable leading these teams in a high-pressure, dynamic &ndash; and global &ndash; environment. Jobs are broadly defined and interact with Product Management, Sales and other groups at Google.</p> <h3>The role: Engineering Director</h3> <p>We''re looking for a highly technical Engineering Director who can drive technical projects and provide leadership for a group 40+ engineers in an innovative and fast paced environment. You will be responsible for the overall planning, execution, and success of the projects.</p> <h4>Qualities we are looking for:</h4> <ul><li>Very high technical competence, track record of strong coding and individual technical accomplishments, and strong academic record.</li><li>Software background, Internet experience.</li><li>Entrepreneurial drive, demonstrated ability to achieve stretch goals in an innovative and fast paced environment.</li><li>5+ years relevant experience managing large, fast-paced and dynamic engineering teams.</li><li>Solid leadership skills, experience building a strong engineering teams.</li><li>Strong project-management skills. Proven track record for product delivery. Able to fit in well within an informal startup environment and to provide hands-on management.</li><li>Establish credibility with smart engineers quickly.</li><li>MS or CS preferred (PhD, a plus).</li></ul>')
GO

GO
SET IDENTITY_INSERT [dbo].[PostingApplications] ON
GO
INSERT INTO [dbo].[PostingApplications]([PostingApplicationId], [TargetPostingId], [SubmittedPostingId], [SubmittedOn], [SubmittedByUserId], [Comment])
  VALUES(9, '1D80CF3E-B5DD-487B-96DB-4F3DC040C2B5', 'B6A1A9CB-BEEF-4F15-AF36-E70E9E86F082', '20090520 11:13:43.0', '49D95127-610B-43CD-9638-929E3BF94072', 'Dear NoRecruiters user. We are very interested in your resume and would like to speak with you.  Please contact us at your earliest convinience

            ')
GO
INSERT INTO [dbo].[PostingApplications]([PostingApplicationId], [TargetPostingId], [SubmittedPostingId], [SubmittedOn], [SubmittedByUserId], [Comment])
  VALUES(10, 'E440CEE0-9148-4E15-B6DD-D637F4760690', '1D80CF3E-B5DD-487B-96DB-4F3DC040C2B5', '20090520 11:14:17.0', 'BB876D54-5932-4414-92AD-EFB7BED21123', '                
    Dear NoRecruiters user. I am very interested in your job posting and would like to speak with you. 
    Please contact me at your earliest convinience

            ')
GO
SET IDENTITY_INSERT [dbo].[PostingApplications] OFF
GO

GO
SET IDENTITY_INSERT [dbo].[PostingTags] ON
GO
INSERT INTO [dbo].[PostingTags]([TagId], [PostingId], [TagText], [SafeText])
  VALUES(1127, 'B6A1A9CB-BEEF-4F15-AF36-E70E9E86F082', 'google', 'google')
GO
INSERT INTO [dbo].[PostingTags]([TagId], [PostingId], [TagText], [SafeText])
  VALUES(1128, 'B6A1A9CB-BEEF-4F15-AF36-E70E9E86F082', 'web20', 'web20')
GO
INSERT INTO [dbo].[PostingTags]([TagId], [PostingId], [TagText], [SafeText])
  VALUES(1129, 'E440CEE0-9148-4E15-B6DD-D637F4760690', 'google', 'google')
GO
INSERT INTO [dbo].[PostingTags]([TagId], [PostingId], [TagText], [SafeText])
  VALUES(1130, 'E440CEE0-9148-4E15-B6DD-D637F4760690', 'c++', 'cplusplus')
GO
INSERT INTO [dbo].[PostingTags]([TagId], [PostingId], [TagText], [SafeText])
  VALUES(1135, '1D80CF3E-B5DD-487B-96DB-4F3DC040C2B5', 'linux', 'linux')
GO
INSERT INTO [dbo].[PostingTags]([TagId], [PostingId], [TagText], [SafeText])
  VALUES(1136, '1D80CF3E-B5DD-487B-96DB-4F3DC040C2B5', 'unix', 'unix')
GO
INSERT INTO [dbo].[PostingTags]([TagId], [PostingId], [TagText], [SafeText])
  VALUES(1137, '1D80CF3E-B5DD-487B-96DB-4F3DC040C2B5', 'basic', 'basic')
GO
INSERT INTO [dbo].[PostingTags]([TagId], [PostingId], [TagText], [SafeText])
  VALUES(1138, '1D80CF3E-B5DD-487B-96DB-4F3DC040C2B5', 'msword', 'msword')
GO
SET IDENTITY_INSERT [dbo].[PostingTags] OFF
GO
GO
INSERT INTO [dbo].[Users]([UserId], [UserName], [Password], [FirstName], [LastName], [PostingId], [UserTypeId], [Email])
  VALUES('49D95127-610B-43CD-9638-929E3BF94072', 'google', 'password', 'google', 'admin', 'B6A1A9CB-BEEF-4F15-AF36-E70E9E86F082', 0, 'jobs@google.com')
GO
INSERT INTO [dbo].[Users]([UserId], [UserName], [Password], [FirstName], [LastName], [PostingId], [UserTypeId], [Email])
  VALUES('BB876D54-5932-4414-92AD-EFB7BED21123', 'sample', 'password', 'sample', 'user', '1D80CF3E-B5DD-487B-96DB-4F3DC040C2B5', 1, 'sample@email.com')
GO

GO
SET IDENTITY_INSERT [dbo].[UserRoles] ON
GO
INSERT INTO [dbo].[UserRoles]([UserRoleId], [RoleName], [UserId])
  VALUES(4, 'company', '49D95127-610B-43CD-9638-929E3BF94072')
GO
SET IDENTITY_INSERT [dbo].[UserRoles] OFF
GO
